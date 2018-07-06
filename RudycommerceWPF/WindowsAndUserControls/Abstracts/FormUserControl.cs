using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RudycommerceWPF.WindowsAndUserControls.Abstracts
{
    public abstract class FormUserControl: MultilingualUserControl
    {
        public delegate void FormSave(ContentControl contentControl);
        public event FormSave SaveEvent;

        public ContentControl thisContentControl { get; set; }

        protected void TriggerSaveEvent()
        {
            SaveEvent(thisContentControl);
        }
    }
}
