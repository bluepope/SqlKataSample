using ConsoleApp1.Samples;

string connectionString = "User ID=test;Password=test;Host=192.168.0.210;Port=3306;Database=testdb;Convert Zero Datetime=True";

//1. DataTable 
//new DataTableSample(connectionString).GetData();

//2. DapperSample
//await new DapperSample(connectionString).GetDataAsync();

//3. EFCore Sample
//await new EFCoreSample(connectionString).GetDataAsync();

//4. SqlKata Sample
var sample = new SqlKataSample(connectionString);


//await sample.GetDataAsync();
await sample.JoinSelectAsync();

//await sample.InsertAsync(new()
//{
//    id = 2,
//    name = "테스트2",
//});

//await sample.UpdateAsync(new()
//{
//    id = 2,
//    name = "테스트3",
//});

//await sample.DeleteAsync(new()
//{
//    id = 2,
//});