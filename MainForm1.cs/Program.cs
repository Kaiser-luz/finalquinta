// Projeto: Sistema de Gerenciamento de Clínica Médica
// Linguagem: C# com Windows Forms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace ClinicaMedica
{
    public class Medico
    {
        public string Nome { get; set; }
        public string Especialidade { get; set; }
        public string CRM { get; set; }

        public override string ToString() => $"{Nome} ({Especialidade}) - CRM: {CRM}";
    }

    public class Paciente
    {
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string CPF { get; set; }

        public override string ToString() => $"{Nome} - CPF: {CPF}";
    }

    public class Consulta
    {
        public Medico Medico { get; set; }
        public Paciente Paciente { get; set; }
        public DateTime DataConsulta { get; set; }

        public override string ToString() => $"{DataConsulta:dd/MM/yyyy} - Médico: {Medico.Nome}, Paciente: {Paciente.Nome}";
    }

    public class Clinica
    {
        public List<Medico> Medicos { get; set; } = new List<Medico>();
        public List<Paciente> Pacientes { get; set; } = new List<Paciente>();
        public List<Consulta> Consultas { get; set; } = new List<Consulta>();

        public void CadastrarMedico(string nome, string especialidade, string crm) =>
            Medicos.Add(new Medico { Nome = nome, Especialidade = especialidade, CRM = crm });

        public void CadastrarPaciente(string nome, DateTime dataNascimento, string cpf) =>
            Pacientes.Add(new Paciente { Nome = nome, DataNascimento = dataNascimento, CPF = cpf });

        public void AgendarConsulta(Medico medico, Paciente paciente, DateTime dataConsulta) =>
            Consultas.Add(new Consulta { Medico = medico, Paciente = paciente, DataConsulta = dataConsulta });

        public List<Consulta> BuscarConsultas(string termo) =>
            Consultas.Where(c => c.Medico.Nome.IndexOf(termo, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 c.Paciente.Nome.IndexOf(termo, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 c.DataConsulta.ToShortDateString().Contains(termo)).ToList();

        public List<Consulta> ListarConsultas() => Consultas;
    }

    public class MainForm : Form
    {
        private Clinica clinica = new Clinica();

        private TextBox txtNomeMedico, txtEspecialidade, txtCRM;
        private TextBox txtNomePaciente, txtCPF;
        private DateTimePicker dtpNascimento, dtpConsulta;
        private ComboBox cmbMedicos, cmbPacientes;
        private TextBox txtBusca;
        private ListBox lstConsultas;

        public MainForm()
        {
            Text = "Sistema de Clínica Médica";
            Size = new Size(800, 600);
            BackColor = Color.White;

            Label lblTitulo = new Label() { Text = "Clínica Médica", Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = Color.Navy, AutoSize = true, Location = new Point(300, 10) };
            Controls.Add(lblTitulo);

            // Cadastro Médico
            Controls.Add(new Label() { Text = "Nome Médico:", Location = new Point(20, 60) });
            txtNomeMedico = new TextBox() { Location = new Point(120, 60), Width = 200 };
            Controls.Add(txtNomeMedico);

            Controls.Add(new Label() { Text = "Especialidade:", Location = new Point(20, 90) });
            txtEspecialidade = new TextBox() { Location = new Point(120, 90), Width = 200 };
            Controls.Add(txtEspecialidade);

            Controls.Add(new Label() { Text = "CRM:", Location = new Point(20, 120) });
            txtCRM = new TextBox() { Location = new Point(120, 120), Width = 200 };
            Controls.Add(txtCRM);

            Button btnCadastrarMedico = new Button() { Text = "Cadastrar Médico", Location = new Point(120, 150), BackColor = Color.LightBlue };
            btnCadastrarMedico.Click += (s, e) =>
            {
                clinica.CadastrarMedico(txtNomeMedico.Text, txtEspecialidade.Text, txtCRM.Text);
                AtualizarComboMedicos();
                MessageBox.Show("Médico cadastrado!");
            };
            Controls.Add(btnCadastrarMedico);

            // Cadastro Paciente
            Controls.Add(new Label() { Text = "Nome Paciente:", Location = new Point(400, 60) });
            txtNomePaciente = new TextBox() { Location = new Point(520, 60), Width = 200 };
            Controls.Add(txtNomePaciente);

            Controls.Add(new Label() { Text = "Nascimento:", Location = new Point(400, 90) });
            dtpNascimento = new DateTimePicker() { Location = new Point(520, 90), Width = 200 };
            Controls.Add(dtpNascimento);

            Controls.Add(new Label() { Text = "CPF:", Location = new Point(400, 120) });
            txtCPF = new TextBox() { Location = new Point(520, 120), Width = 200 };
            Controls.Add(txtCPF);

            Button btnCadastrarPaciente = new Button() { Text = "Cadastrar Paciente", Location = new Point(520, 150), BackColor = Color.LightBlue };
            btnCadastrarPaciente.Click += (s, e) =>
            {
                clinica.CadastrarPaciente(txtNomePaciente.Text, dtpNascimento.Value, txtCPF.Text);
                AtualizarComboPacientes();
                MessageBox.Show("Paciente cadastrado!");
            };
            Controls.Add(btnCadastrarPaciente);

            // Agendar Consulta
            Controls.Add(new Label() { Text = "Médico:", Location = new Point(20, 200) });
            cmbMedicos = new ComboBox() { Location = new Point(120, 200), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.Add(cmbMedicos);

            Controls.Add(new Label() { Text = "Paciente:", Location = new Point(20, 230) });
            cmbPacientes = new ComboBox() { Location = new Point(120, 230), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.Add(cmbPacientes);

            Controls.Add(new Label() { Text = "Data Consulta:", Location = new Point(20, 260) });
            dtpConsulta = new DateTimePicker() { Location = new Point(120, 260), Width = 200 };
            Controls.Add(dtpConsulta);

            Button btnAgendar = new Button() { Text = "Agendar Consulta", Location = new Point(120, 290), BackColor = Color.LightBlue };
            btnAgendar.Click += (s, e) =>
            {
                if (cmbMedicos.SelectedItem is Medico medico && cmbPacientes.SelectedItem is Paciente paciente)
                {
                    clinica.AgendarConsulta(medico, paciente, dtpConsulta.Value);
                    MessageBox.Show("Consulta agendada!");
                    AtualizarListaConsultas();
                }
            };
            Controls.Add(btnAgendar);

            // Lista e busca
            Controls.Add(new Label() { Text = "Buscar:", Location = new Point(400, 200) });
            txtBusca = new TextBox() { Location = new Point(460, 200), Width = 200 };
            Controls.Add(txtBusca);

            Button btnBuscar = new Button() { Text = "Buscar Consultas", Location = new Point(670, 200), BackColor = Color.LightBlue };
            btnBuscar.Click += (s, e) =>
            {
                lstConsultas.Items.Clear();
                foreach (var c in clinica.BuscarConsultas(txtBusca.Text))
                    lstConsultas.Items.Add(c);
            };
            Controls.Add(btnBuscar);

            Button btnListar = new Button() { Text = "Listar Consultas", Location = new Point(460, 230), BackColor = Color.LightBlue };
            btnListar.Click += (s, e) => AtualizarListaConsultas();
            Controls.Add(btnListar);

            lstConsultas = new ListBox() { Location = new Point(400, 270), Width = 360, Height = 200 };
            Controls.Add(lstConsultas);
        }

        private void AtualizarComboMedicos()
        {
            cmbMedicos.Items.Clear();
            foreach (var m in clinica.Medicos)
                cmbMedicos.Items.Add(m);
        }

        private void AtualizarComboPacientes()
        {
            cmbPacientes.Items.Clear();
            foreach (var p in clinica.Pacientes)
                cmbPacientes.Items.Add(p);
        }

        private void AtualizarListaConsultas()
        {
            lstConsultas.Items.Clear();
            foreach (var c in clinica.ListarConsultas())
                lstConsultas.Items.Add(c);
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
