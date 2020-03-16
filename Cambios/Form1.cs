namespace Cambios
{
    using Servicos;
    using Modelos;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using System.Threading.Tasks;
    using System;

    public partial class Form1 : Form
    {
        //atributos
        private NetworkService networkService;
        private ApiService apiService;
        private List<Rate> Rates;
        private DialogService dialogService;
        private DataService dataService;

        public Form1()
        {
            InitializeComponent();
            networkService = new NetworkService();
            apiService = new ApiService();
            dialogService = new DialogService();
            dataService = new DataService();
            LoadRates();
        }

        private async void LoadRates()
        {
            bool load;

            lblResultado.Text = "A atualizar taxas...";

            var connection = networkService.CheckConnection();//verificar se existe ligação à internet

            if (!connection.IsSuccess)//caso não exista ligação à internet
            {
                LoadLocalRates();
                load = false;
            }
            else//se existir ligação à internet
            {
                await LoadApiRates();
                load = true;
            }

            if (Rates.Count == 0)
            {
                lblResultado.Text = "Não há ligação à internet" + Environment.NewLine +
                    "e não foram previamente carregadas taxas." + Environment.NewLine +
                    "Tente mais tarde!";

                lblStatus.Text = "Primeira inicialização deverá ter ligação à internet";

                return;
            }

            cbOrigem.DataSource = Rates;
            cbOrigem.DisplayMember = "Name";

            cbDestino.BindingContext = new BindingContext();//limpar o binding da comboBoxDestino

            cbDestino.DataSource = Rates;
            cbDestino.DisplayMember = "Name";

            lblResultado.Text = "Taxas atualizadas...";

            if (load)
            {
                lblStatus.Text = string.Format($"Taxas carregadas da internet em {DateTime.Now:F}");
            }
            else
            {
                lblStatus.Text = string.Format("Taxas carregadas da base de dados.");
            }

            progressBar1.Value = 100;
            btnConverter.Enabled = true;
            btnTroca.Enabled = true;
        }

        //Carregar lista a partir da base de dados
        private void LoadLocalRates()
        {
            Rates = dataService.GetData();
        }

        //Carregar lista a partir da api e atualizar os dados na base de dados
        private async Task LoadApiRates()
        {
            progressBar1.Value = 0;

            var response = await apiService.GetRates("https://cambiosrafa.azurewebsites.net", "/api/rates");//Ligar e carregar dados da api

            Rates = (List<Rate>)response.Result;

            dataService.DeleteData();
            dataService.SaveData(Rates);
        }

        private void btnConverter_Click(object sender, EventArgs e)
        {
            Converter();
        }

        //Validações + conversão de moedas
        private void Converter()
        {
            if (string.IsNullOrEmpty(txtValor.Text))
            {
                dialogService.ShowMessage("Erro", "Insira um valor a converter");
                return;
            }

            decimal valor;

            if (!decimal.TryParse(txtValor.Text, out valor))
            {
                dialogService.ShowMessage("Erro de conversão", "Valor terá que ser numérico");
                return;
            }

            if (cbOrigem.SelectedItem == null)
            {
                dialogService.ShowMessage("Erro", "Tem que escolher uma moeda a converter");
                return;
            }

            if (cbDestino.SelectedItem == null)
            {
                dialogService.ShowMessage("Erro", "Tem que escolher uma moeda de destino para converter");
                return;
            }

            var taxaOrigem = (Rate)cbOrigem.SelectedItem;
            var taxaDestino = (Rate)cbDestino.SelectedItem;
            var valorConvertido = valor / (decimal)taxaOrigem.TaxRate * (decimal)taxaDestino.TaxRate;

            lblResultado.Text = string.Format($"{taxaOrigem.Code} {valor:N2} = {taxaDestino.Code} {valorConvertido:N2}");
        }

        private void btnTroca_Click(object sender, EventArgs e)
        {
            Trocar();
        }

        private void Trocar()
        {
            var aux = cbOrigem.SelectedItem;
            cbOrigem.SelectedItem = cbDestino.SelectedItem;
            cbDestino.SelectedItem = aux;
            Converter();
        }
    }
}
