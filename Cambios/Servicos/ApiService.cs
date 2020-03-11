namespace Cambios.Servicos
{
    using Modelos;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ApiService
    {
        /// <summary>
        /// Ligar e carregar dados da api
        /// </summary>
        /// <param name="urlBase">endereço base da api</param>
        /// <param name="controller">controlador da api</param>
        /// <returns>Retorna uma Task<Response></returns>
        public async Task<Response> GetRates(string urlBase, string controller)
        {
            try
            {
                var client = new HttpClient();//conexão externa via http
                client.BaseAddress = new Uri(urlBase);//endereço base da api

                var response = await client.GetAsync(controller);//controlador da api (pasta onde estão os rates)

                var result = await response.Content.ReadAsStringAsync();//carregar tudo o que está na api

                if (!response.IsSuccessStatusCode)//se der erro
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result
                    };
                }

                var rates = JsonConvert.DeserializeObject<List<Rate>>(result);//converter json numa lista de dados do tipo Rate

                return new Response
                {
                    IsSuccess = true,
                    Result = rates
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
