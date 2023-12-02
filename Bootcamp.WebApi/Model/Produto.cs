using Google.Cloud.Firestore;

namespace Bootcamp.WebApi.Model
{
    [FirestoreData]
    public class Produto
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public string IdFornecedor { get; set; }
        [FirestoreProperty]
        public string Nome { get; set; }
        [FirestoreProperty]
        public string Descricao { get; set; }
        [FirestoreProperty]
        public int Quantidade { get; set; }
        [FirestoreProperty]
        public string preco { get; set; }
        [FirestoreProperty]
        public bool Ativo { get; set; }
    }
}
