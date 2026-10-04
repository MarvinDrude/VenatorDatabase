using Venator.Storage;

Console.WriteLine("Testing realm");

var storageEngine = StorageEngineBootstrapper.OpenOrCreate("database.venator", new StorageEngineOptions()
{

});

_ = "";
