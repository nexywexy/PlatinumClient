namespace MaterialSkin.Animations
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    internal class AnimationManager
    {
        private const double MIN_VALUE = 0.0;
        private const double MAX_VALUE = 1.0;
        private readonly List<double> animationProgresses;
        private readonly List<Point> animationSources;
        private readonly List<AnimationDirection> animationDirections;
        private readonly List<object[]> animationDatas;
        private readonly System.Windows.Forms.Timer animationTimer;

        public event AnimationFinished OnAnimationFinished;

        public event AnimationProgress OnAnimationProgress;

        public AnimationManager(bool singular = true)
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer {
                Interval = 5,
                Enabled = false
            };
            this.animationTimer = timer;
            this.animationProgresses = new List<double>();
            this.animationSources = new List<Point>();
            this.animationDirections = new List<AnimationDirection>();
            this.animationDatas = new List<object[]>();
            this.Increment = 0.03;
            this.SecondaryIncrement = 0.03;
            this.AnimationType = MaterialSkin.Animations.AnimationType.Linear;
            this.InterruptAnimation = true;
            this.Singular = singular;
            if (this.Singular)
            {
                this.animationProgresses.Add(0.0);
                this.animationSources.Add(new Point(0, 0));
                this.animationDirections.Add(AnimationDirection.In);
            }
            this.animationTimer.Tick += new EventHandler(this.AnimationTimerOnTick);
        }

        private void AnimationTimerOnTick(object sender, EventArgs eventArgs)
        {
            for (int i = 0; i < this.animationProgresses.Count; i++)
            {
                this.UpdateProgress(i);
                if (!this.Singular)
                {
                    if ((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.InOutIn) && (this.animationProgresses[i] == 1.0))
                    {
                        this.animationDirections[i] = AnimationDirection.InOutOut;
                    }
                    else if ((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.InOutRepeatingIn) && (this.animationProgresses[i] == 0.0))
                    {
                        this.animationDirections[i] = AnimationDirection.InOutRepeatingOut;
                    }
                    else if ((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.InOutRepeatingOut) && (this.animationProgresses[i] == 0.0))
                    {
                        this.animationDirections[i] = AnimationDirection.InOutRepeatingIn;
                    }
                    else if ((((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.In) && (this.animationProgresses[i] == 1.0)) || ((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.Out) && (this.animationProgresses[i] == 0.0))) || ((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.InOutOut) && (this.animationProgresses[i] == 0.0)))
                    {
                        this.animationProgresses.RemoveAt(i);
                        this.animationSources.RemoveAt(i);
                        this.animationDirections.RemoveAt(i);
                        this.animationDatas.RemoveAt(i);
                    }
                }
                else if ((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.InOutIn) && (this.animationProgresses[i] == 1.0))
                {
                    this.animationDirections[i] = AnimationDirection.InOutOut;
                }
                else if ((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.InOutRepeatingIn) && (this.animationProgresses[i] == 1.0))
                {
                    this.animationDirections[i] = AnimationDirection.InOutRepeatingOut;
                }
                else if ((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.InOutRepeatingOut) && (this.animationProgresses[i] == 0.0))
                {
                    this.animationDirections[i] = AnimationDirection.InOutRepeatingIn;
                }
            }
            if (this.OnAnimationProgress != null)
            {
                this.OnAnimationProgress(this);
            }
        }

        private void DecrementProgress(int index)
        {
            List<double> list;
            int num;
            (list = this.animationProgresses)[num = index] = list[num] - (((((AnimationDirection) this.animationDirections[index]) == AnimationDirection.InOutOut) || (((AnimationDirection) this.animationDirections[index]) == AnimationDirection.InOutRepeatingOut)) ? this.SecondaryIncrement : this.Increment);
            if (this.animationProgresses[index] < 0.0)
            {
                this.animationProgresses[index] = 0.0;
                for (int i = 0; i < this.GetAnimationCount(); i++)
                {
                    if ((((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.InOutIn) || (((AnimationDirection) this.animationDirections[i]) == AnimationDirection.InOutRepeatingIn)) || ((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.InOutRepeatingOut) || ((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.InOutOut) && (this.animationProgresses[i] != 0.0)))) || ((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.Out) && (this.animationProgresses[i] != 0.0)))
                    {
                        return;
                    }
                }
                this.animationTimer.Stop();
                if (this.OnAnimationFinished != null)
                {
                    this.OnAnimationFinished(this);
                }
            }
        }

        public int GetAnimationCount() => 
            this.animationProgresses.Count;

        public object[] GetData()
        {
            if (!this.Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }
            if (this.animationDatas.Count == 0)
            {
                throw new Exception("Invalid animation");
            }
            return this.animationDatas[0];
        }

        public object[] GetData(int index)
        {
            if (index >= this.animationDatas.Count)
            {
                throw new IndexOutOfRangeException("Invalid animation index");
            }
            return this.animationDatas[index];
        }

        public AnimationDirection GetDirection()
        {
            if (!this.Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }
            if (this.animationDirections.Count == 0)
            {
                throw new Exception("Invalid animation");
            }
            return this.animationDirections[0];
        }

        public AnimationDirection GetDirection(int index)
        {
            if (index >= this.animationDirections.Count)
            {
                throw new IndexOutOfRangeException("Invalid animation index");
            }
            return this.animationDirections[index];
        }

        public double GetProgress()
        {
            if (!this.Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }
            if (this.animationProgresses.Count == 0)
            {
                throw new Exception("Invalid animation");
            }
            return this.GetProgress(0);
        }

        public double GetProgress(int index)
        {
            if (index >= this.GetAnimationCount())
            {
                throw new IndexOutOfRangeException("Invalid animation index");
            }
            switch (this.AnimationType)
            {
                case MaterialSkin.Animations.AnimationType.Linear:
                    return AnimationLinear.CalculateProgress(this.animationProgresses[index]);

                case MaterialSkin.Animations.AnimationType.EaseInOut:
                    return AnimationEaseInOut.CalculateProgress(this.animationProgresses[index]);

                case MaterialSkin.Animations.AnimationType.EaseOut:
                    return AnimationEaseOut.CalculateProgress(this.animationProgresses[index]);

                case MaterialSkin.Animations.AnimationType.CustomQuadratic:
                    return AnimationCustomQuadratic.CalculateProgress(this.animationProgresses[index]);
            }
            throw new NotImplementedException("The given AnimationType is not implemented");
        }

        public Point GetSource()
        {
            if (!this.Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }
            if (this.animationSources.Count == 0)
            {
                throw new Exception("Invalid animation");
            }
            return this.animationSources[0];
        }

        public Point GetSource(int index)
        {
            if (index >= this.GetAnimationCount())
            {
                throw new IndexOutOfRangeException("Invalid animation index");
            }
            return this.animationSources[index];
        }

        private void IncrementProgress(int index)
        {
            List<double> list;
            int num;
            (list = this.animationProgresses)[num = index] = list[num] + this.Increment;
            if (this.animationProgresses[index] > 1.0)
            {
                this.animationProgresses[index] = 1.0;
                for (int i = 0; i < this.GetAnimationCount(); i++)
                {
                    if ((((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.InOutIn) || (((AnimationDirection) this.animationDirections[i]) == AnimationDirection.InOutRepeatingIn)) || ((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.InOutRepeatingOut) || ((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.InOutOut) && (this.animationProgresses[i] != 1.0)))) || ((((AnimationDirection) this.animationDirections[i]) == AnimationDirection.In) && (this.animationProgresses[i] != 1.0)))
                    {
                        return;
                    }
                }
                this.animationTimer.Stop();
                if (this.OnAnimationFinished != null)
                {
                    this.OnAnimationFinished(this);
                }
            }
        }

        public bool IsAnimating() => 
            this.animationTimer.Enabled;

        public void SetData(object[] data)
        {
            if (!this.Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }
            if (this.animationDatas.Count == 0)
            {
                throw new Exception("Invalid animation");
            }
            this.animationDatas[0] = data;
        }

        public void SetDirection(AnimationDirection direction)
        {
            if (!this.Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }
            if (this.animationProgresses.Count == 0)
            {
                throw new Exception("Invalid animation");
            }
            this.animationDirections[0] = direction;
        }

        public void SetProgress(double progress)
        {
            if (!this.Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }
            if (this.animationProgresses.Count == 0)
            {
                throw new Exception("Invalid animation");
            }
            this.animationProgresses[0] = progress;
        }

        public void StartNewAnimation(AnimationDirection animationDirection, object[] data = null)
        {
            this.StartNewAnimation(animationDirection, new Point(0, 0), data);
        }

        public void StartNewAnimation(AnimationDirection animationDirection, Point animationSource, object[] data = null)
        {
            if (this.IsAnimating() && !this.InterruptAnimation)
            {
                goto Label_01CD;
            }
            if (this.Singular && (this.animationDirections.Count > 0))
            {
                this.animationDirections[0] = animationDirection;
            }
            else
            {
                this.animationDirections.Add(animationDirection);
            }
            if (this.Singular && (this.animationSources.Count > 0))
            {
                this.animationSources[0] = animationSource;
            }
            else
            {
                this.animationSources.Add(animationSource);
            }
            if (!this.Singular || (this.animationProgresses.Count <= 0))
            {
                switch (this.animationDirections[this.animationDirections.Count - 1])
                {
                    case AnimationDirection.In:
                    case AnimationDirection.InOutIn:
                    case AnimationDirection.InOutRepeatingIn:
                        this.animationProgresses.Add(0.0);
                        goto Label_0164;

                    case AnimationDirection.Out:
                    case AnimationDirection.InOutOut:
                    case AnimationDirection.InOutRepeatingOut:
                        this.animationProgresses.Add(1.0);
                        goto Label_0164;
                }
                throw new Exception("Invalid AnimationDirection");
            }
        Label_0164:
            if (this.Singular && (this.animationDatas.Count > 0))
            {
                if (data == null)
                {
                }
                this.animationDatas[0] = new object[0];
            }
            else
            {
                if (data == null)
                {
                }
                this.animationDatas.Add(new object[0]);
            }
        Label_01CD:
            this.animationTimer.Start();
        }

        public void UpdateProgress(int index)
        {
            switch (this.animationDirections[index])
            {
                case AnimationDirection.In:
                case AnimationDirection.InOutIn:
                case AnimationDirection.InOutRepeatingIn:
                    this.IncrementProgress(index);
                    break;

                case AnimationDirection.Out:
                case AnimationDirection.InOutOut:
                case AnimationDirection.InOutRepeatingOut:
                    this.DecrementProgress(index);
                    break;

                default:
                    throw new Exception("No AnimationDirection has been set");
            }
        }

        public bool InterruptAnimation { get; set; }

        public double Increment { get; set; }

        public double SecondaryIncrement { get; set; }

        public MaterialSkin.Animations.AnimationType AnimationType { get; set; }

        public bool Singular { get; set; }

        public delegate void AnimationFinished(object sender);

        public delegate void AnimationProgress(object sender);
    }
}

