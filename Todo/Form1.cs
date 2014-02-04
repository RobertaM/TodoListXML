using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace WindowsFormsApplication6
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            listView1.CheckBoxes = true;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            loadTaskFile();
            loadContactFile();
        }

        LinkedList<ToDoListItem> list = new LinkedList<ToDoListItem>();
        LinkedList<ContactItem> list2 = new LinkedList<ContactItem>();

        private void Form_listView()
        {
            listView1.Items.Add(new ListViewItem());
        }

        private void deleteTask_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listView1.SelectedItems)
            {
                ToDoListItem todoItem = eachItem.Tag as ToDoListItem;
                list.Remove(todoItem);
                listView1.Items.Remove(eachItem);
            }

            for (int i = 0; i < list.Count; i++)
                list.ElementAt(i).number = i + 1;

            foreach (ListViewItem eachItem in listView1.Items)
                eachItem.SubItems[1].Text = (eachItem.Tag as ToDoListItem).number.ToString();
            
            writeToFileTasks();
        }

        public void refreshTasksView()
        {

            listView1.Items.Clear();
 
            for (int i = 0; i < list.Count(); i++)
            {
               if ((comboBox1.Text != String.Empty && list.ElementAt(i).type != comboBox1.Text) && comboBox1.Text != "All")
                {
                    continue;
                }
                DateTime date;
                if (checkBox1.Checked && DateTime.TryParse(list.ElementAt(i).date, out date))
                {
                    var difference = monthCalendar1.SelectionRange.Start - date;
                    if (difference.TotalDays > 0)
                    {
                        continue;
                    }
                }
                var item = new ListViewItem();                
                item.Text = "";
                item.Tag = list.ElementAt(i);
                item.Checked = list.ElementAt(i).check;
                item.SubItems.Add(list.ElementAt(i).number.ToString());
                item.SubItems.Add(list.ElementAt(i).type);
                item.SubItems.Add(list.ElementAt(i).todo);
                item.SubItems.Add(list.ElementAt(i).date);

                updateColors(list.ElementAt(i), item);
                                
                listView1.Items.Add(item);
                listView1.Tag = item;
                
            }
        }

        private void updateColors(ToDoListItem toDoListItem, ListViewItem item)
        {
            DateTime dt;
            if (DateTime.TryParse(toDoListItem.date, out dt))
            {
                TimeSpan difference = dt - DateTime.Now;
                item.SubItems.Add(difference.Days.ToString());

                if (item.Checked)
                {
                    item.BackColor = Color.White;
                    item.ForeColor = Color.Gray;
                }
                else
                {
                    item.ForeColor = Color.Black;
                    if (difference.TotalDays > 1)
                    {
                        item.BackColor = Color.Green;
                    }
                    else if (difference.TotalDays >= 0)
                    {
                        item.BackColor = Color.Yellow;
                    }
                    else
                    {
                        item.BackColor = Color.Red;
                    }
                }
            }
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ListViewItem item = e.Item as ListViewItem;
            (item.Tag as ToDoListItem).check = item.Checked;
            updateColors(item.Tag as ToDoListItem, item); 

            writeToFileTasks();
        }

        private void editTask_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ToDoListItem todo = listView1.SelectedItems[0].Tag as ToDoListItem;
                Form2 secondForm = new Form2(todo);
                if (secondForm.ShowDialog(this) == DialogResult.OK)
                {
                    refreshTasksView();
                    writeToFileTasks();
                }
            }
            else
            {
                MessageBox.Show("Select one task");
            }
         }

        private void editContact_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count == 1)
            {
                ContactItem contact = listView2.SelectedItems[0].Tag as ContactItem;
                Form3 thirdForm = new Form3(contact);
                if (thirdForm.ShowDialog(this) == DialogResult.OK)
                {
                    refreshContactsView();
                    writeToFileContacts();
                }
            }
            else
            {
                MessageBox.Show("Select one contact");
            }
        }

        private void addContact_Click(object sender, EventArgs e)
        {           
                ContactItem contact = new ContactItem( list2.Count()+1, "","","","");
                Form3 thirdForm = new Form3(contact);
                if (thirdForm.ShowDialog(this) == DialogResult.OK)
                {
                    var item = new ListViewItem();
                    item.Tag = contact;
                    item.SubItems.Add(contact.number.ToString());
                    item.SubItems.Add(contact.name);
                    item.SubItems.Add(contact.surname);
                    item.SubItems.Add(contact.phoneNumber);
                    item.SubItems.Add(contact.address);
                    listView2.Items.Add(item);
                    list2.AddLast(contact);
                }
                writeToFileContacts();
        }

        public void refreshContactsView()
        {
            foreach (ListViewItem item in listView2.Items)
            {
                listView2.Items.Remove(item);
            }
            for (int i = 0; i <= list2.Count() - 1; i++)
            {
                var item = new ListViewItem();
                item.Tag = list2.ElementAt(i);
                item.SubItems.Add(list2.ElementAt(i).number.ToString());
                item.SubItems.Add(list2.ElementAt(i).name);
                item.SubItems.Add(list2.ElementAt(i).surname);
                item.SubItems.Add(list2.ElementAt(i).phoneNumber);
                item.SubItems.Add(list2.ElementAt(i).address);
                listView2.Items.Add(item);
            }
        }

        private void addTask_Click(object sender, EventArgs e)
        {
            ToDoListItem todo = new ToDoListItem(false, list.Count() + 1, "", "", "");
            Form2 secondForm = new Form2(todo);
            if (secondForm.ShowDialog(this) == DialogResult.OK)
            {
                var item = new ListViewItem();
                item.Tag = todo;
                item.Checked = todo.check;
                item.SubItems.Add(todo.number.ToString());
                item.SubItems.Add(todo.type);
                item.SubItems.Add(todo.todo);
                if (!String.IsNullOrEmpty(todo.date))
                {
                    DateTime temp;
                    if (DateTime.TryParse(todo.date, out temp))
                    {
                        DateTime dt = DateTime.Parse(todo.date);
                        TimeSpan difference = dt - DateTime.Now;
                        item.SubItems.Add(todo.date);
                        item.SubItems.Add(difference.Days.ToString());
                    }
                }
                listView1.Items.Add(item);
                list.AddLast(todo);
                refreshTasksView();
                writeToFileTasks();               
            }
        }

        private void loadTaskFile()
        {
            ToDoListItem todo = new ToDoListItem(false, 0, "", "", "");
            XmlDocument doc = new XmlDocument();
            
            if (new FileInfo("todo.xml").Length < 10)
            {
                return;
            }
            
            doc.Load("todo.xml");
            XmlNodeList elemList = doc.GetElementsByTagName("ToDo");
            list.Clear();

            foreach(XmlNode element in elemList)
            {
                string check = element["Check"].InnerText;
                string taskNumber = element["TaskNumber"].InnerText;
                string taskType = element["TaskType"].InnerText;
                string task = element["Task"].InnerText;
                string date = element["Date"].InnerText;
                int num = Convert.ToInt32(taskNumber);
                bool ch = Convert.ToBoolean(check);                              
                ToDoListItem ToDoItem = new ToDoListItem( ch, num, taskType, task, date);
                list.AddLast(ToDoItem);                
            }
            
            refreshTasksView();
            doc.Save("todo.xml");
        }

        private void loadContactFile()
        {
           
            XmlDocument doc = new XmlDocument();

            if (new FileInfo("contact.xml").Length < 10)
            {
                return;
            }

            doc.Load("contact.xml");
            XmlNodeList elemList = doc.GetElementsByTagName("Contact");
            list2.Clear();

            foreach (XmlNode element in elemList)
            {
                string number = element["Number"].InnerText;
                string name = element["Name"].InnerText;
                string surname = element["Surname"].InnerText;
                string phone = element["Phone"].InnerText;
                string address = element["Address"].InnerText;
                
                int num = Convert.ToInt32(number);

                ContactItem contact = new ContactItem(num ,name, surname, phone, address);
                list2.AddLast(contact);
            }

            refreshContactsView();
            doc.Save("contact.xml");
        }

        private void writeToFileTasks()
        {
            XmlWriter writer = XmlWriter.Create("todo.xml");
            using (writer)
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("ToDos");
                foreach (var element in list)
                {
                    
                    writer.WriteStartElement("ToDo");
                    writer.WriteElementString("Check", element.check.ToString());
                    writer.WriteElementString("TaskNumber", element.number.ToString());
                    writer.WriteElementString("TaskType", element.type);
                    writer.WriteElementString("Task", element.todo);
                    writer.WriteElementString("Date", element.date);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void writeToFileContacts()
        {
            XmlWriter writer = XmlWriter.Create("contact.xml");
            using (writer)
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Contacts");
                foreach (var element in list2)
                {
                    writer.WriteStartElement("Contact");
                    writer.WriteElementString("Number", element.number.ToString());
                    writer.WriteElementString("Name", element.name);
                    writer.WriteElementString("Surname", element.surname);
                    writer.WriteElementString("Phone", element.phoneNumber);
                    writer.WriteElementString("Address", element.address);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void deleteContact_Click(object sender, EventArgs e) 
        {
            foreach (ListViewItem eachItem in listView2.SelectedItems)
            {
                ListView.SelectedIndexCollection sel = listView2.SelectedIndices;
                ListViewItem selItem = listView2.Items[sel[0]];
                int delItem = Convert.ToInt32(selItem.SubItems[1].Text);
                list2.Remove(list2.ElementAt(delItem - 1));
                listView2.Items.Remove(eachItem);
                for (int i = 0; i <= list2.Count() - 1; i++)
                {
                    if (delItem - 1 <= i)
                    {
                        list2.ElementAt(i).number -= 1;
                    }
                }
                refreshContactsView();
                writeToFileContacts();
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            refreshTasksView();
        }
       
        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            if (checkBox1.Checked)
            {
                refreshTasksView();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            refreshTasksView();
        }

    }
    
    public class ToDoListItem
    {
        public bool check { get; set; }
        public int number { get; set; }
        public string type { get; set; }
        public string todo { get; set; }
        public string date { get; set; }

        public ToDoListItem(bool Check, int Number, string Type, string Todo, string Date)
        {
            check = Check;
            number = Number;
            type = Type;
            todo = Todo;
            date = Date;
        }
    }
    public class ContactItem
    {        
        public int number { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string phoneNumber { get; set; }
        public string address { get; set; }

    
        public ContactItem( int number, string name, string surname, string phoneNumber, string address)
        {
            this.number = number;
            this.name = name;
            this.surname = surname;
            this.phoneNumber = phoneNumber;
            this.address = address;
        }

    }
}


