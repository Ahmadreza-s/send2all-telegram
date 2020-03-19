using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TeleSharp.TL;
using TLSharp.Core;

namespace SendToAllTelegram
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //enter data here from my.telegram.org
        string apiHash = "";
        int apiId = 0;


        string phone = "";
        private string hash = "";
        private TelegramClient client;
        private List<TLUser> Contacts=new List<TLUser>();
        string message = "";


        private int index = 0;

        private async void button1_Click(object sender, EventArgs e)
        {
            phone = mobileTxt.Text;
            client = new TelegramClient(apiId, apiHash);
            await client.ConnectAsync();
            hash = await client.SendCodeRequestAsync(phone);
            button2.Enabled = true;

        }


        private async void button2_Click(object sender, EventArgs e)
        {
            await client.MakeAuthAsync(phone, hash, codeTxt.Text);
            var result = await client.GetContactsAsync();
            Contacts = result.users.lists
                .Where(x => x.GetType() == typeof(TLUser))
                .Cast<TLUser>().ToList();

            Contacts.ForEach(user =>
            {
                listBox1.Items.Add(user.first_name + " " + user.last_name);
            });
   
            groupBox1.Text = "Contacts (0/" + listBox1.Items.Count + ")";
            button3.Enabled = true;
            button1.Enabled = mobileTxt.Enabled = false;


        }

        void NextUser()
        {
            if (Contacts.Count <= index)
            {
                MessageBox.Show(this, "Operation Completed !", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button1.Enabled = mobileTxt.Enabled = true;
                listBox1.ClearSelected();
                return;
            }
            var name = $"{Contacts[index].first_name} {Contacts[index].last_name}";
            var sm = new SendMessage(Contacts[index].id, string.Format(message,name), client);
            sm.OnComplete += () =>
            {
                groupBox1.Text = "Contacts (" + (index + 1) + "/" + listBox1.Items.Count + ")";
                Thread.Sleep(1000);
                listBox1.SelectedIndex = index;
                index++;
                NextUser();
            };
        }

        
        private void button3_Click(object sender, EventArgs e)
        {
            button2.Enabled = codeTxt.Enabled = mobileTxt.Enabled = false;

            message = richTextBox1.Text;
            var name = $"{Contacts[index].first_name} {Contacts[index].last_name}";
            var sm = new SendMessage(Contacts[index].id, string.Format(message, name), client);
            sm.OnComplete += () =>
            {
                groupBox1.Text = "Contacts (1/" + listBox1.Items.Count + ")";
                listBox1.SelectedIndex = 0;
                Thread.Sleep(500);
                index = 1;
                NextUser();
            };
        }
    }
}
