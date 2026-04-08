using Modern.Forms;

namespace ControlGallery.Panels
{
    public class DataGridViewPanel : BasePanel
    {
        public DataGridViewPanel ()
        {
            Controls.Add (new Label { Text = "DataGridView - Row Selection (click headers to sort, double-click cells to edit)", Left = 10, Top = 10, Width = 600 });

            var dgv1 = new DataGridView {
                Left = 10,
                Top = 30,
                Width = 700,
                Height = 250
            };

            dgv1.Columns.Add ("Name", 150);
            dgv1.Columns.Add ("Age", 60);
            dgv1.Columns.Add ("City", 120);
            dgv1.Columns.Add ("Occupation", 150);
            dgv1.Columns.Add ("Email", 200);

            dgv1.Rows.Add ("Alice Johnson", "32", "New York", "Engineer", "alice@example.com");
            dgv1.Rows.Add ("Bob Smith", "45", "Los Angeles", "Designer", "bob@example.com");
            dgv1.Rows.Add ("Carol Williams", "28", "Chicago", "Teacher", "carol@example.com");
            dgv1.Rows.Add ("David Brown", "51", "Houston", "Doctor", "david@example.com");
            dgv1.Rows.Add ("Eve Davis", "39", "Phoenix", "Lawyer", "eve@example.com");
            dgv1.Rows.Add ("Frank Miller", "22", "Philadelphia", "Student", "frank@example.com");
            dgv1.Rows.Add ("Grace Wilson", "36", "San Antonio", "Architect", "grace@example.com");
            dgv1.Rows.Add ("Henry Moore", "48", "San Diego", "Manager", "henry@example.com");
            dgv1.Rows.Add ("Ivy Taylor", "31", "Dallas", "Analyst", "ivy@example.com");
            dgv1.Rows.Add ("Jack Anderson", "55", "San Jose", "Director", "jack@example.com");
            dgv1.Rows.Add ("Karen Thomas", "42", "Austin", "Consultant", "karen@example.com");
            dgv1.Rows.Add ("Leo Jackson", "27", "Jacksonville", "Developer", "leo@example.com");
            dgv1.Rows.Add ("Mia White", "34", "Fort Worth", "Scientist", "mia@example.com");
            dgv1.Rows.Add ("Noah Harris", "60", "Columbus", "Professor", "noah@example.com");
            dgv1.Rows.Add ("Olivia Martin", "25", "Charlotte", "Intern", "olivia@example.com");

            dgv1.SelectedRowIndex = 2;

            Controls.Add (dgv1);

            Controls.Add (new Label { Text = "DataGridView - Data Binding", Left = 10, Top = 300, Width = 300 });

            var dgv3 = new DataGridView {
                Left = 10,
                Top = 320,
                Width = 500,
                Height = 200
            };

            var products = new List<Product> {
                new Product { Name = "Widget", Price = 9.99, Quantity = 100, Category = "Hardware" },
                new Product { Name = "Gadget", Price = 24.95, Quantity = 50, Category = "Electronics" },
                new Product { Name = "Doohickey", Price = 4.50, Quantity = 200, Category = "Hardware" },
                new Product { Name = "Thingamajig", Price = 15.00, Quantity = 75, Category = "Electronics" },
                new Product { Name = "Whatchamacallit", Price = 7.25, Quantity = 150, Category = "Misc" },
                new Product { Name = "Contraption", Price = 49.99, Quantity = 10, Category = "Electronics" },
                new Product { Name = "Gizmo", Price = 12.50, Quantity = 80, Category = "Hardware" },
                new Product { Name = "Doodad", Price = 3.75, Quantity = 300, Category = "Misc" }
            };

            dgv3.DataSource = products;

            Controls.Add (dgv3);
        }

        private sealed class Product
        {
            public string Name { get; set; } = string.Empty;
            public double Price { get; set; }
            public int Quantity { get; set; }
            public string Category { get; set; } = string.Empty;
        }
    }
}
