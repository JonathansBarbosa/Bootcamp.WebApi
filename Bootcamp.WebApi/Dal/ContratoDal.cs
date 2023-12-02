using Bootcamp.WebApi.Dal.Repositories;
using Bootcamp.WebApi.Dal.Repositories;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Bootcamp.WebApi.Dal
{
    public class Contrato : IContrato
    {
        string projectId;
        FirestoreDb fireStoreDb;
        public Contrato()
        {
            /*Caminho do arquivo baixado do firebase ou gcloud, colocar na raiz do projeto*/
            string arquivoApiKey = @"bootcamp-9a19e-firebase-adminsdk-s2jtb-fa0f95bc26.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", arquivoApiKey);
            projectId = "project-Bootcamp";
            fireStoreDb = FirestoreDb.Create(projectId);
        }
        public async Task<List<Model.Contrato>> GetContratos()
        {
            try
            {
                Query ContratoQuery = fireStoreDb.Collection("Contrato");
                QuerySnapshot ContratoQuerySnaphot = await ContratoQuery.GetSnapshotAsync();
                List<Model.Contrato> listaContrato = new List<Model.Contrato>();
                foreach (DocumentSnapshot documentSnapshot in ContratoQuerySnaphot.Documents)
                {
                    if (documentSnapshot.Exists)
                    {
                        Dictionary<string, object> city = documentSnapshot.ToDictionary();
                        string json = JsonConvert.SerializeObject(city);
                        Model.Contrato novoContrato = JsonConvert.DeserializeObject<Model.Contrato>(json);
                        novoContrato.Id = documentSnapshot.Id;
                        listaContrato.Add(novoContrato);
                    }
                }
                List<Model.Contrato> listaContratoOrdenada = listaContrato.OrderBy(x => x.DataVencimento).ToList();
                return listaContratoOrdenada;
            }
            catch (Exception ex)
            {
                var erro = ex.Message;
                throw;
            }
        }
        public async Task<Model.Contrato> GetContrato(string id)
        {
            try
            {
                DocumentReference docRef = fireStoreDb.Collection("Contrato").Document(id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Model.Contrato contrato = snapshot.ConvertTo<Model.Contrato>();
                    contrato.Id = snapshot.Id;
                    return contrato;
                }
                else
                {
                    return new Model.Contrato();
                }
            }
            catch
            {
                throw;
            }
        }
        public string AddContrato(Model.Contrato contrato)
        {
            try
            {
                CollectionReference colRef = fireStoreDb.Collection("Contrato");
                var id = colRef.AddAsync(contrato).Result.Id;
                var shardRef = colRef.Document(id.ToString());
                shardRef.UpdateAsync("Id", id);
                return id;
            }
            catch
            {
                return "Error";
            }
        }
        public async void UpdateContrato(Model.Contrato contrato)
        {
            try
            {
                DocumentReference contratoRef = fireStoreDb.Collection("Contrato").Document(contrato.Id);
                await contratoRef.SetAsync(contrato, SetOptions.Overwrite);
            }
            catch
            {
                throw;
            }
        }
        public async void DeleteContrato(string id)
        {
            try
            {
                DocumentReference contratoRef = fireStoreDb.Collection("Contrato").Document(id);
                await contratoRef.DeleteAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}

