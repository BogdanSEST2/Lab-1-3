using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bogdan_SEST_2_22._Winform_Lab_4
{
    public partial class Form1 : Form
    {
        private MenuStrip menuStrip;
        private ToolStripMenuItem aboutMenuItem;
        private Button selectFolderButton;
        private TextBox folderPathTextBox;
        private ListBox directoriesListBox;
        private DataGridView filesDataGridView;
        private Button processFilesButton;
        private Button duplicateFolderButton;

        public Form1() => InitializeComponent();

        private void InitializeComponent()
        {
            this.ClientSize = new Size((int)(Screen.PrimaryScreen.WorkingArea.Width * 0.75),
                                       (int)(Screen.PrimaryScreen.WorkingArea.Height * 0.75));
            this.FormBorderStyle = FormBorderStyle.FixedSingle; 
            this.StartPosition = FormStartPosition.CenterScreen; 
            this.Text = "File and Folder Manager";

            menuStrip = new MenuStrip();
            aboutMenuItem = new ToolStripMenuItem("About");
            aboutMenuItem.Click += AboutMenuItem_Click;
            menuStrip.Items.Add(aboutMenuItem);
            this.MainMenuStrip = menuStrip;

            selectFolderButton = new Button
            {
                Text = "Выбрать папку",
                Location = new Point(10, 30)
            };
            selectFolderButton.Click += SelectFolderButton_Click;

            folderPathTextBox = new TextBox
            {
                Location = new Point(10, 60),
                Width = this.ClientSize.Width - 20,
                ReadOnly = true
            };

            directoriesListBox = new ListBox
            {
                Location = new Point(10, 90),
                Size = new Size(200, 500)
            };
            directoriesListBox.DoubleClick += DirectoriesListBox_DoubleClick;

            filesDataGridView = new DataGridView
            {
                Location = new Point(220, 90),
                Size = new Size(500, 500)
            };
            filesDataGridView.Columns.Add("FileName", "Название");
            filesDataGridView.Columns.Add("LastModified", "Дата последней модификации");
            filesDataGridView.Columns.Add("FileSize", "Количество байт");
            filesDataGridView.DoubleClick += FilesDataGridView_DoubleClick;

            processFilesButton = new Button
            {
                Text = "Запустить обработку файлов",
                Location = new Point(10, 300),
                Visible = false
            };
            processFilesButton.Click += ProcessFilesButton_Click;

            duplicateFolderButton = new Button
            {
                Text = "Продублировать папку",
                Location = new Point(150, 300),
                Visible = true
            };
            duplicateFolderButton.Click += DuplicateFolderButton_Click;

            this.Controls.Add(menuStrip);
            this.Controls.Add(selectFolderButton);
            this.Controls.Add(folderPathTextBox);
            this.Controls.Add(directoriesListBox);
            this.Controls.Add(filesDataGridView);
            this.Controls.Add(processFilesButton);
            this.Controls.Add(duplicateFolderButton);
            this.MainMenuStrip = menuStrip;
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Разработчик: Богдан Цыбин из SEST-2-22", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SelectFolderButton_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderDialog = new();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderDialog.SelectedPath;
                folderPathTextBox.Text = selectedPath;
                DisplayDirectories(selectedPath);
                DisplayFiles(selectedPath);
                processFilesButton.Visible = true;
            }
        }

        private void DisplayDirectories(string path)
        {
            directoriesListBox.Items.Clear();
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                directoriesListBox.Items.Add(Path.GetFileName(directory));
            }
        }

        private void DisplayFiles(string path)
        {
            filesDataGridView.Rows.Clear();
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                FileInfo fileInfo = new(file);
                filesDataGridView.Rows.Add(fileInfo.Name, fileInfo.LastWriteTime, fileInfo.Length);
            }
        }

        private void DirectoriesListBox_DoubleClick(object sender, EventArgs e)
        {
            if (directoriesListBox.SelectedItem != null)
            {
                string selectedDirectory = directoriesListBox.SelectedItem.ToString();
                string fullPath = Path.Combine(folderPathTextBox.Text, selectedDirectory);
                DirectoryInfo dirInfo = new(fullPath);

                Form directoryInfoForm = new()
                {
                    Text = "Информация о папке",
                    ClientSize = new Size(300, 150)
                };

                Label nameLabel = new() { Text = $"Название: {dirInfo.Name}", Location = new Point(10, 10) };
                Label lastModifiedLabel = new() { Text = $"Дата последней модификации: {dirInfo.LastWriteTime}", Location = new Point(10, 40) };

                directoryInfoForm.Controls.Add(nameLabel);
                directoryInfoForm.Controls.Add(lastModifiedLabel);
                directoryInfoForm.ShowDialog();
            }
        }

        private void FilesDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (filesDataGridView.CurrentRow != null)
            {
                string fileName = filesDataGridView.CurrentRow.Cells[0].Value.ToString();
                string fullPath = Path.Combine(folderPathTextBox.Text, fileName);

                DialogResult result = MessageBox.Show($"Вы хотите продублировать файл {fileName}?", "Дублирование файла", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string newFileName = Path.Combine(folderPathTextBox.Text, $"{Path.GetFileNameWithoutExtension(fileName)}_copy{Path.GetExtension(fileName)}");
                    File.Copy(fullPath, newFileName);
                    DisplayFiles(folderPathTextBox.Text);
                }
            }
        }

        private async void ProcessFilesButton_Click(object sender, EventArgs e)
        {
            Random random = new();
            foreach (DataGridViewRow row in filesDataGridView.Rows)
            {
                _ = row.Cells[0].Value.ToString();
                int delaySeconds = random.Next(1, filesDataGridView.Rows.Count + 1);

                await Task.Delay(delaySeconds * 1000);

                if (filesDataGridView.Columns["Delay"] == null)
                {
                    filesDataGridView.Columns.Add("Delay", "Задержка (сек)");
                }

                row.Cells["Delay"].Value = delaySeconds;
            }
        }

        private void DuplicateFolderButton_Click(object sender, EventArgs e)
        {
            if (directoriesListBox.SelectedItem != null)
            {
                string selectedDirectory = directoriesListBox.SelectedItem.ToString();
                string fullPath = Path.Combine(folderPathTextBox.Text, selectedDirectory);

                DialogResult result = MessageBox.Show($"Ты уверен что хочешь продублировать папку {selectedDirectory}?", "Дублирование папки", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string newFolderPath = Path.Combine(folderPathTextBox.Text, $"{selectedDirectory}_copy");
                    DirectoryCopy(fullPath, newFolderPath, true);
                    DisplayDirectories(folderPathTextBox.Text);
                }
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Исходный каталог не существует или не может быть найден: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
    }
}