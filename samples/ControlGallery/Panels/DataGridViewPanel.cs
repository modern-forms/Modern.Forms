using Modern.Forms;

namespace ControlGallery.Panels
{
    public class DataGridViewPanel : BasePanel
    {
        public DataGridViewPanel ()
        {
            Controls.Add (new Label { Text = "DataGridView - Row Selection", Left = 10, Top = 10, Width = 300 });

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

            Controls.Add (new Label { Text = "DataGridView - Cell Selection, No Headers", Left = 10, Top = 300, Width = 300 });

            var dgv2 = new DataGridView {
                Left = 10,
                Top = 320,
                Width = 500,
                Height = 200,
                RowSelectionMode = false,
                ColumnHeadersVisible = false
            };

            dgv2.Columns.Add ("Column 1", 120);
            dgv2.Columns.Add ("Column 2", 120);
            dgv2.Columns.Add ("Column 3", 120);
            dgv2.Columns.Add ("Column 4", 120);

            for (var i = 0; i < 10; i++)
                dgv2.Rows.Add ($"Cell {i},0", $"Cell {i},1", $"Cell {i},2", $"Cell {i},3");

            Controls.Add (dgv2);
        }
    }
}
