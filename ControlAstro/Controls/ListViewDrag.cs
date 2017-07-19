using System;
using System.Drawing;
using System.Windows.Forms;

namespace ControlAstro.Controls
{
    public class ListViewDrag : ListView
    {
        public ListViewDrag()
        {
            this.AllowDrop = true;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            int targetIndex = InsertionMark.Index;
            if (targetIndex == -1)
            {
                return;
            }
            if (InsertionMark.AppearsAfterItem)
            {
                targetIndex++;
            }

            ListViewItem draggedItem = (ListViewItem)drgevent.Data.GetData(typeof(ListViewItem));
            BeginUpdate();
            var item = (ListViewItem)draggedItem.Clone();
            Items.Insert(targetIndex, item);
            Items.Remove(draggedItem);
            EndUpdate();
            Items[Items.IndexOf(item)].Selected = true;
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
            Point ptScreen = new Point(drgevent.X, drgevent.Y);
            Point pt = PointToClient(ptScreen);
            ListViewItem item = GetItemAt(pt.X, pt.Y);

            int targetIndex = InsertionMark.NearestIndex(pt);
            if (targetIndex > -1)
            {
                Rectangle itemBounds = GetItemRect(targetIndex);
                if (pt.X > itemBounds.Left + (itemBounds.Width / 2))
                {
                    InsertionMark.AppearsAfterItem = true;
                }
                else
                {
                    InsertionMark.AppearsAfterItem = false;
                }
            }
            InsertionMark.Index = targetIndex;
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
            drgevent.Effect = drgevent.AllowedEffect;
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
            InsertionMark.Index = -1;
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            base.OnItemDrag(e);
            DoDragDrop(e.Item, DragDropEffects.Move);
        }
    }
}
