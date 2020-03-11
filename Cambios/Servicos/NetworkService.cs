namespace Cambios.Servicos
{
    using Modelos;
    using System.Net;

    public class NetworkService
    {
        /// <summary>
        /// Verificar se existe ligação à internet
        /// </summary>
        /// <returns>Retorna objeto do tipo Response</returns>
        public Response CheckConnection()
        {
            var client = new WebClient();

            try
            {
                using (client.OpenRead("http://clients3.google.com/generate_204"))//ping ao servidor da google
                {
                    return new Response//há ligação à internet
                    {
                        IsSuccess = true
                    };
                }
            }
            catch
            {
                return new Response//não há ligação à internet
                {
                    IsSuccess = false,
                    Message = "Configure a sua ligação à Internet"
                };
            }
        }
    }
}
