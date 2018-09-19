using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Windows.Forms;

namespace ResxComparer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs ea)
        {
            string directory;
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Resx Files (*.resx)|*.resx";
            dialog.Title = "First";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;

            ResXResourceSet resourceSet1 = null;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                directory = Directory.GetParent(dialog.FileName).FullName;
                try
                {
                    using (FileStream file = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        resourceSet1 = new ResXResourceSet(file);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                return;
            }

            dialog.Title = "Second";
            ResXResourceSet resourceSet2 = null;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (FileStream file = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        resourceSet2 = new ResXResourceSet(file);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                return;
            }

            var diff1 = new Dictionary<string, string>();
            foreach (DictionaryEntry e1 in resourceSet1)
            {
                var found = false;
                foreach (DictionaryEntry e2 in resourceSet2)
                {
                    if (e1.Key.ToString() == e2.Key.ToString())
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    diff1.Add(e1.Key.ToString(), e1.Value.ToString());
                }
            }

            var diff2 = new Dictionary<string, string>();
            foreach (DictionaryEntry e2 in resourceSet2)
            {
                var found = false;
                foreach (DictionaryEntry e1 in resourceSet1)
                {
                    if (e1.Key.ToString() == e2.Key.ToString())
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    diff2.Add(e2.Key.ToString(), e2.Value.ToString());
                }
            }

            if (diff1.Count == 0 && diff2.Count == 0)
            {
                MessageBox.Show("No difference", "ResxComparer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var file1 = Path.Combine(directory, "OnlyInFirst.resx");
            if (diff1.Count > 0)
            {
                using (var writer = new ResXResourceWriter(file1))
                {
                    foreach (var e in Sort(diff1))
                    {
                        writer.AddResource(e.Key, e.Value);
                    }
                    writer.Generate();
                }
            }
            else if (File.Exists(file1))
            {
                File.Delete(file1);
            }

            var file2 = Path.Combine(directory, "OnlyInSecond.resx");
            if (diff2.Count > 0)
            {
                using (var writer = new ResXResourceWriter(file2))
                {
                    foreach (var e in Sort(diff2))
                    {
                        writer.AddResource(e.Key, e.Value);
                    }
                    writer.Generate();
                }
            }
            else if (File.Exists(file2))
            {
                File.Delete(file2);
            }
            MessageBox.Show("Done", "ResxComparer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            System.Diagnostics.Process.Start(directory);
        }

        private void button2_Click(object sender, EventArgs ea)
        {
            string directory;
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Resx Files (*.resx)|*.resx";
            dialog.Title = "First";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;

            ResXResourceSet resourceSet1 = null;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                directory = Directory.GetParent(dialog.FileName).FullName;
                try
                {
                    using (FileStream file = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        resourceSet1 = new ResXResourceSet(file);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                return;
            }

            dialog.Title = "Second";
            ResXResourceSet resourceSet2 = null;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (FileStream file = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        resourceSet2 = new ResXResourceSet(file);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                return;
            }

            var hasNew = false; 
            var dic = new Dictionary<string, string>();
            foreach (DictionaryEntry e1 in resourceSet1)
            {
                dic.Add(e1.Key.ToString(), e1.Value.ToString());
            }
            
            foreach (DictionaryEntry e2 in resourceSet2)
            {
                if (dic.ContainsKey(e2.Key.ToString()))
                {
                    if (dic[e2.Key.ToString()] != e2.Value.ToString())
                    {
                        dic[e2.Key.ToString()] = e2.Value.ToString();
                        hasNew = true;
                    }
                }
                else
                {
                    dic.Add(e2.Key.ToString(), e2.Value.ToString());
                    hasNew = true;
                }
            }

            if (hasNew)
            {
                var file = Path.Combine(directory, "Combined.resx");
                using (var writer = new ResXResourceWriter(file))
                {
                    foreach (var e in Sort(dic))
                    {
                        writer.AddResource(e.Key, e.Value);
                    }
                    writer.Generate();
                }
                MessageBox.Show("Done", "ResxComparer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Diagnostics.Process.Start(directory);
            }
            else
            {
                MessageBox.Show("Nothing new", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static Dictionary<string, string> Sort(Dictionary<string, string> dic)
        {
            var lst = new List<KeyValuePair<string, string>>(dic);
            lst.Sort(delegate (KeyValuePair<string, string> s1, KeyValuePair<string, string> s2)
            {
                return s1.Key.CompareTo(s2.Key);
            });

            var newDic = new Dictionary<string, string>();
            foreach (var entry in lst)
            {
                newDic.Add(entry.Key, entry.Value);
            }
            return newDic;
        }
    }
}
