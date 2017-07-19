using System.Drawing;
using System.Windows.Forms;

namespace ControlAstro.Controls
{
    public partial class TipPanel : UserControl
    {
        public Image ImgInfo 
        {
            set
            {
                imgInfo.Image = value;
            }
            get
            {
                return imgInfo.Image;
            }
        }

        public string InfoText
        {
            set
            {
                lblInfo.Text = value;
            }
            get
            {
                return lblInfo.Text;
            }
        }

        public TipPanel()
        {
            InitializeComponent();
        }
    }
}
