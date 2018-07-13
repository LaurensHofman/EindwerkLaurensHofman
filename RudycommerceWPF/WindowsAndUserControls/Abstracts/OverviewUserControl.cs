using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RudycommerceWPF.WindowsAndUserControls.Abstracts
{
    public abstract class OverviewUserControl: MultilingualUserControl
    {
        /// <summary>
        /// Creates instance of the Generic Form type, with ID to load the Item in the form
        /// </summary>
        /// <typeparam name="FormUC"></typeparam>
        /// <param name="ID"></param>
        protected void ShowUpdateForm<FormUC>(int ID) where FormUC: FormUserControl, new()
        {
            // https://stackoverflow.com/questions/1852837/is-there-a-generic-constructor-with-parameter-constraint-in-c

            FormUC form = (FormUC)Activator.CreateInstance(typeof(FormUC), (int)ID);

            form.UpdateEvent += Updated;

            UpdateFormWindow win = new UpdateFormWindow(form);
            win.Show();
        }

        public abstract void LoadDataGridData();

        private void Updated()
        {
            LoadDataGridData();
        }

        protected Window GetParentWindow()
        {
            NavigationWindow myWindow = (NavigationWindow)Window.GetWindow(this);

            return myWindow;
        }

        protected void RefreshGrid(object sender, RoutedEventArgs e)
        {
            LoadDataGridData();
        }

        protected abstract void Delete(object sender, RoutedEventArgs e);

        protected abstract void Update(object sender, RoutedEventArgs e);

        protected abstract void OpenForm(object sender, RoutedEventArgs e);
    }
}
