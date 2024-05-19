using Simulateur65xx.FW;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Simulateur65xx.OB
{
    public class Animation
    {
        public enum ANIM
        {
            PC_2_MEMORY = 1,
            MEMORY_2_PC,
            MEMORY_2_OPCODE,
            OPCODE_2_PC,
            MEMORY_2_PARAM,
            PARAM_2_PC,
            PARAM_2_OPCODE,
            OPCODE_2_A,
            A_2_PS,
            PS_2_PC,
        }
        
        public int AnimationStep = 0;
        public int Speed = 0;
        public int AnimSpeed = 4;
        public List<Point> AnimPoints;
        public bool RunningAnimation=false;

        public Timer timerAnimation;
        private PictureBox pbLIGHT;
        private ComboBox cbSPEED;

        public Animation(PictureBox lightSquare, ComboBox speedComboBox)
        {
            pbLIGHT = lightSquare;
            cbSPEED = speedComboBox;

            timerAnimation = new Timer();
            timerAnimation.Tick += timerAnimation_Tick;
        }

        public static List<Point> PC_2_MEMORY = new List<Point>
        {
            new Point(502, 289),
            new Point(394,289),
            new Point(394,352),
            new Point(1112,352),
        };

        public static List<Point> MEMORY_2_PC = new List<Point>
        {
            new Point(1112,352),
            new Point(394,352),
            new Point(394,289),
            new Point(502, 289),
        };

        public static List<Point> MEMORY_2_OPCODE = new List<Point>
        {
            new Point(1112,352),
            new Point(339,352),
            new Point(339,403),
        };

        public static List<Point> MEMORY_2_PARAM = new List<Point>
        {
            new Point(1112,352),
            new Point(470,352),
            new Point(470,403),
        };

        public static List<Point> OPCODE_2_PC = new List<Point>
        {
            new Point(339,403),
            new Point(339,352),
            new Point(394,352),
            new Point(394,289),
            new Point(502, 289),
        };

        public static List<Point> OPCODE_2_A = new List<Point>
        {
            new Point(339,403),
            new Point(339,352),
            new Point(394,352),
            new Point(394,37),
            new Point(287, 37),
        };

        public static List<Point> A_2_PS = new List<Point>
        {
            new Point(287, 37),
            new Point(502, 37),
        };

        public static List<Point> PS_2_PC = new List<Point>
        {
            new Point(502, 37),
            new Point(394, 37),
            new Point(394, 289),
            new Point(502, 289),
        };


        public static List<Point> PARAM_2_PC = new List<Point>
        {
            new Point(470,403),
            new Point(470,352),
            new Point(394,352),
            new Point(394,289),
            new Point(502, 289),
        };

        public static List<Point> PARAM_2_OPCODE = new List<Point>
        {
            new Point(470,403),
            new Point(470,352),
            new Point(339,352),
            new Point(339,403),
        };

        public void SetSpeed()
        {
            Speed = (6-cbSPEED.SelectedIndex)*8;
            AnimSpeed = 3 + cbSPEED.SelectedIndex * 6;
        }

        public void SetStartPoint(List<Point> anim)
        {
            pbLIGHT.Location = anim[0];
        }

        public void RunAnimation()
        {
            if (AnimationStep == 0)
            {
                pbLIGHT.Location = AnimPoints[0];
                AnimationStep++;
            }
            else
            if (AnimationStep < AnimPoints.Count)
            {
                int x = pbLIGHT.Location.X;
                int y = pbLIGHT.Location.Y;

                int fromX = AnimPoints[AnimationStep-1].X;
                int fromY = AnimPoints[AnimationStep-1].Y;

                int toX = AnimPoints[AnimationStep].X;
                int toY = AnimPoints[AnimationStep].Y;

                if (fromX != toX)
                {
                    if (fromX > toX)
                    {
                        x -= AnimSpeed;
                        if (x < toX)
                        {
                            x = toX;
                            AnimationStep++;
                        }
                    }
                    else
                    {
                        x += AnimSpeed;
                        if (x > toX)
                        {
                            x = toX;
                            AnimationStep++;
                        }
                    }
                }
                if (fromY != toY)
                {
                    if (fromY > toY)
                    {
                        y -= AnimSpeed;
                        if (y < toY)
                        {
                            y = toY;
                            AnimationStep++;
                        }
                    }
                    else
                    {
                        y += AnimSpeed;
                        if (y > toY)
                        {
                            y = toY;
                            AnimationStep++;
                        }
                    }
                }

                pbLIGHT.Location = new Point(x,y);

            }
            else
            {
                RunningAnimation = false;
                return;
            }
            
            timerAnimation.Enabled = true;
        }

        public void timerAnimation_Tick(object sender, EventArgs e)
        {
            timerAnimation.Enabled = false;
            RunAnimation();
        }

        public void StartAnimation(ANIM anim)
        {
            if (Main.isClosing) return;
            if (timerAnimation.Enabled) return;
            RunningAnimation=true;
            SetSpeed();
            switch (anim)
            {
                case ANIM.PC_2_MEMORY:
                    AnimPoints = PC_2_MEMORY;
                    break;
                case ANIM.MEMORY_2_PC:
                    AnimPoints = MEMORY_2_PC;
                    break;
                case ANIM.MEMORY_2_OPCODE:
                    AnimPoints = MEMORY_2_OPCODE;
                    break;
                case ANIM.OPCODE_2_PC:
                    AnimPoints = OPCODE_2_PC;
                    break;
                case ANIM.PARAM_2_PC:
                    AnimPoints = PARAM_2_PC;
                    break;
                case ANIM.MEMORY_2_PARAM:
                    AnimPoints = MEMORY_2_PARAM;
                    break;
                case ANIM.PARAM_2_OPCODE:
                    AnimPoints = PARAM_2_OPCODE;
                    break;
                case ANIM.OPCODE_2_A:
                    AnimPoints = OPCODE_2_A;
                    break;
                case ANIM.A_2_PS:
                    AnimPoints = A_2_PS;
                    break;
                case ANIM.PS_2_PC:
                    AnimPoints = PS_2_PC;
                    break;
                default:
                    return;
            }

            AnimationStep = 0;
            timerAnimation.Interval = Speed;
            timerAnimation.Enabled = true;

            while (RunningAnimation)
            {
                Timing.Wait(100);
            }
        }
    }
}