namespace MaterialSkin.Controls
{
    using MaterialSkin;
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Forms;

    public class MaterialContextMenuStrip : ContextMenuStrip, IMaterialControl
    {
        internal AnimationManager animationManager;
        internal Point animationSource;
        private ToolStripItemClickedEventArgs delayesArgs;

        public event ItemClickStart OnItemClickStart;

        public MaterialContextMenuStrip()
        {
            AnimationManager.AnimationProgress progress = null;
            AnimationManager.AnimationFinished finished = null;
            base.Renderer = new MaterialToolStripRender();
            AnimationManager manager = new AnimationManager(false) {
                Increment = 0.07,
                AnimationType = AnimationType.Linear
            };
            this.animationManager = manager;
            if (progress == null)
            {
                progress = sender => base.Invalidate();
            }
            this.animationManager.OnAnimationProgress += progress;
            if (finished == null)
            {
                finished = sender => this.OnItemClicked(this.delayesArgs);
            }
            this.animationManager.OnAnimationFinished += finished;
            base.BackColor = this.SkinManager.GetApplicationBackgroundColor();
        }

        protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
        {
            if ((e.ClickedItem != null) && !(e.ClickedItem is ToolStripSeparator))
            {
                if (e == this.delayesArgs)
                {
                    base.OnItemClicked(e);
                }
                else
                {
                    this.delayesArgs = e;
                    if (this.OnItemClickStart != null)
                    {
                        this.OnItemClickStart(this, e);
                    }
                    this.animationManager.StartNewAnimation(AnimationDirection.In, null);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs mea)
        {
            base.OnMouseUp(mea);
            this.animationSource = mea.Location;
        }

        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager =>
            MaterialSkinManager.Instance;

        [Browsable(false)]
        public MaterialSkin.MouseState MouseState { get; set; }

        public delegate void ItemClickStart(object sender, ToolStripItemClickedEventArgs e);
    }
}

