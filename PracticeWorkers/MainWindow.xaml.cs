using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.IO;

namespace PracticeWorkers
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            JobCreate();
            WorkCreate();
        }


        //добавляет нового сотрудника
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Worker worker = new Worker();
            worker.family = Family.Text;
            worker.name = Name.Text;
            worker.fathname = FathName.Text;
            worker.post = Convert.ToString(Post.SelectedItem);
            worker.id = WorkList.Items.Count;
            
            var deserWork = WorkDeserialization();
            deserWork.Add(worker);
            WorkSerialization(deserWork);

            string family = Family.Text;
            string name = Name.Text;
            string father = FathName.Text;
            string post = Convert.ToString(Post.SelectedItem);

            Grid grid = new Grid();
            grid.MinHeight = 50;
            grid.MaxHeight = 50;

            Label label = new Label();
            label.Content = family + " " + name + " " + father;
            Label textPost = new Label();
            textPost.Content = post;

            RowDefinition row = new RowDefinition();
            row.MinHeight = 25;
            RowDefinition row2 = new RowDefinition();
            row.MinHeight = 25;
            grid.RowDefinitions.Add(row);
            grid.RowDefinitions.Add(row2);
            RowDefinition row1 = new RowDefinition();
            row1.MaxHeight = 50;

            Grid.SetRow(label, 0);
            Grid.SetRow(textPost, 1);
            grid.Children.Add(textPost);
            grid.Children.Add(label);
            grid.ShowGridLines = true;

            ListBoxItem boxItem = new ListBoxItem();
            SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Black);
            boxItem.BorderBrush = solidColorBrush;
            boxItem.Content = grid;
            boxItem.BorderThickness = new Thickness(1);

            WorkList.Items.Add(boxItem);

            Family.Text = "";
            Name.Text = "";
            FathName.Text = "";
            Post.SelectedItem = null;
        }

        //заполняет поля справа при выборе сотрудника
        private void WorkList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var workers = WorkDeserialization();
                Family.Text = workers[WorkList.SelectedIndex].family;
                Name.Text = workers[WorkList.SelectedIndex].name;
                FathName.Text = workers[WorkList.SelectedIndex].fathname;
                Post.SelectedItem = workers[WorkList.SelectedIndex].post;
            } catch (System.ArgumentOutOfRangeException)
            {
                WorkList.SelectedIndex = 0;
            }
        }

        //возвращает список работников
        public static List<Worker> WorkDeserialization(string jsonFile = @".\WorkersData.json")
        {
            var jsonString = "";
            if (File.Exists(jsonFile))
            {
                jsonString = File.ReadAllText(jsonFile);
            } else
            {
                File.Create(jsonFile);
                jsonString = File.ReadAllText(jsonFile);
            }
            
            var WorkDeserList = JsonConvert.DeserializeObject<List<Worker>>(jsonString);
            if (WorkDeserList is null)
            {
                WorkDeserList = new List<Worker>();
            }
            return WorkDeserList;
        }

        //сохраняет список работников
        public static void WorkSerialization(List<Worker> wrks, string jsonFile = @".\WorkersData.json")
        {
            File.WriteAllText(jsonFile, JsonConvert.SerializeObject(wrks));
        }

        //получает список должностей
        public static List<Post> JobDeserialization(string jsonJob = @".\JobList.json")
        {
            var jsonFile = "";
            if (File.Exists(jsonJob))
            {
                jsonFile = File.ReadAllText(jsonJob);
            } else
            {
                File.Create(jsonJob);
                jsonFile = File.ReadAllText(jsonJob);
            }
            var deserJob = JsonConvert.DeserializeObject<List<Post>>(jsonFile);

            return deserJob;
        }

        //выводит список должностей в комбобокс
        public void JobCreate()
        {
            var jobs = JobDeserialization();
            foreach (var item in jobs)
            {
                Post.Items.Add(Convert.ToString(item.job));
            }
            
        }

        //выводит список работников
        public void WorkCreate(string json = @".\WorkersData.json")
        {
            var workers = WorkDeserialization(json);
            foreach (var item in workers)
            {
                var family = item.family;
                var name = item.name;
                var father = item.fathname;
                var post = item.post;

                Grid grid = new Grid();
                grid.MinHeight = 50;
                grid.MaxHeight = 50;

                Label label = new Label();
                label.Content = family + " " + name + " " + father;
                Label textPost = new Label();
                textPost.Content = post;

                RowDefinition row = new RowDefinition();
                row.MinHeight = 25;
                RowDefinition row2 = new RowDefinition();
                row.MinHeight = 25;
                ColumnDefinition column = new ColumnDefinition();
                grid.RowDefinitions.Add(row);
                grid.RowDefinitions.Add(row2);
                RowDefinition row1 = new RowDefinition();
                row1.MaxHeight = 50;

                Grid.SetRow(label, 0);
                Grid.SetRow(textPost, 1);
                grid.Children.Add(textPost);
                grid.Children.Add(label);
                grid.ShowGridLines = true;

                ListBoxItem boxItem = new ListBoxItem();
                SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Black);
                boxItem.BorderBrush = solidColorBrush;
                boxItem.Content = grid;
                boxItem.BorderThickness = new Thickness(1);

                WorkList.Items.Add(boxItem);
            }
        }

        //удаляет запись из списка рабочих
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var delitem = WorkList.SelectedItem;
            var delindex = WorkList.SelectedIndex;
            var workers = WorkDeserialization();
            workers.RemoveAt(delindex);
            WorkSerialization(workers);
            WorkList.Items.Remove(delitem);
            WorkList.SelectedItem = null;
            WorkList.SelectedIndex = 0;
            Family.Text = "";
            Name.Text = "";
            FathName.Text = "";
            Post.SelectedItem = null;
        }

        //сохраняет введённые данные в json и обновляет список
        private void Change_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var workers = WorkDeserialization();
                var id = WorkList.SelectedIndex;
                workers[id].family = Family.Text;
                workers[id].name = Name.Text;
                workers[id].fathname = FathName.Text;
                workers[id].post = Post.Text;
                WorkList.SelectedItem = null;
                WorkSerialization(workers);
                WorkList.Items.Clear();
                WorkCreate();
            } catch(System.ArgumentOutOfRangeException)
            {
                MessageBox.Show("Выберите сотрудника для изменения", "Warning!", MessageBoxButton.OK);
            }
            Family.Text = "";
            Name.Text = "";
            FathName.Text = "";
            Post.Text = "";
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".json";
            dialog.Filter = "JSON |*.json";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                string filename = dialog.FileName;
                WorkList.Items.Clear();
                WorkCreate(filename);
            }
        }
    }
}
