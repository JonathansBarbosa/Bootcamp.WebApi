using Bootcamp.WebApi.Dal.Repositories;
using Bootcamp.WebApi.Dal.Repositories;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Bootcamp.WebApi.Dal
{
    public class Produto : IProduto
    {
        string projectId;
        FirestoreDb fireStoreDb;
        public Produto()
        {
            /*Caminho do arquivo baixado do firebase ou gcloud, colocar na raiz do projeto*/
            string arquivoApiKey = @"bootcamp-9a19e-firebase-adminsdk-s2jtb-fa0f95bc26.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", arquivoApiKey);
            projectId = "project-Bootcamp";
            fireStoreDb = FirestoreDb.Create(projectId);
        }
        public async Task<List<Model.Produto>> GetProdutos()
        {
            try
            {
                Query ProdutoQuery = fireStoreDb.Collection("Produto");
                QuerySnapshot produtoQuerySnaphot = await ProdutoQuery.GetSnapshotAsync();
                List<Model.Produto> listaProduto = new List<Model.Produto>();
                foreach (DocumentSnapshot documentSnapshot in produtoQuerySnaphot.Documents)
                {
                    if (documentSnapshot.Exists)
                    {
                        Dictionary<string, object> city = documentSnapshot.ToDictionary();
                        string json = JsonConvert.SerializeObject(city);
                        Model.Produto novoProduto = JsonConvert.DeserializeObject<Model.Produto>(json);
                        novoProduto.Id = documentSnapshot.Id;
                        listaProduto.Add(novoProduto);
                    }
                }
                List<Model.Produto> listaProdutoOrdenada = listaProduto.OrderBy(x => x.Nome).ToList();
                return listaProdutoOrdenada;
            }
            catch (Exception ex)
            {
                var erro = ex.Message;
                throw;
            }
        }
        public async Task<Model.Produto> GetProduto(string id)
        {
            try
            {
                DocumentReference docRef = fireStoreDb.Collection("Produto").Document(id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Model.Produto produto = snapshot.ConvertTo<Model.Produto>();
                    produto.Id = snapshot.Id;
                    return produto;
                }
                else
                {
                    return new Model.Produto();
                }
            }
            catch
            {
                throw;
            }
        }
        public string AddProduto(Model.Produto produto)
        {
            try
            {
                CollectionReference colRef = fireStoreDb.Collection("Produto");
                var id = colRef.AddAsync(produto).Result.Id;
                var shardRef = colRef.Document(id.ToString());
                shardRef.UpdateAsync("Id", id);
                return id;
            }
            catch
            {
                return "Error";
            }
        }
        public async void UpdateProduto(Model.Produto produto)
        {
            try
            {
                DocumentReference produtoRef = fireStoreDb.Collection("Produto").Document(produto.Id);
                await produtoRef.SetAsync(produto, SetOptions.Overwrite);
            }
            catch
            {
                throw;
            }
        }
        public async void DeleteProduto(string id)
        {
            try
            {
                DocumentReference ProdutoRef = fireStoreDb.Collection("Produto").Document(id);
                await ProdutoRef.DeleteAsync();
            }
            catch
            {
                throw;
            }
        }

    }
}
