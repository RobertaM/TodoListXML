using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication6
{
    public partial class Form2 : Form
    {
        ToDoListItem item = null;
        public Form2(ToDoListItem item)
        {
            InitializeComponent();
            this.item = item;
            edit();
            TaskTypes2.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public void edit()
        {
            ToDo2.Text = item.todo;
            date2.Text = item.date;
            TaskTypes2.Text = item.type;
        }

        private void monthCalendar3_DateChanged(object sender, DateRangeEventArgs e)
        {
            date2.MaxLength = 1;
            date2.Text = monthCalendar3.SelectionRange.Start.ToShortDateString();
        }

        private void edit2_Click(object sender, EventArgs e)
        {
            DateTime result;
            if (DateTime.TryParse(date2.Text, out result))
            {
                item.type = TaskTypes2.Text;
                item.todo = ToDo2.Text;
                item.date = date2.Text;

                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Select date please");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            item.type = TaskTypes2.Text;
            item.todo = ToDo2.Text;
            item.date = date2.Text;
        }
       

    }
}
