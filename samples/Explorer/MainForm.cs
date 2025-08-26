using Sistema;
using Sistema.Windowns.Form;
using Sistema.Data.SqlClient;

namespace CadastroClienteApp
{
    public partial class MainForm : Form
{
    public MainForm()
{
    InitializeComponent();
}

private void btnSalvar_Click(object sender Event Args e)
{
     SalvarDadosCliente(txtNome.Text,txtEndereco.Text,txtTelefone.Text,txtEmail.Text)
