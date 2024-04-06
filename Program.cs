using System;
using Npgsql;
public class Program
{
    public static void Main()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Выберите действие:"); 
            Console.WriteLine("1. Просмотреть все типы машин");
            Console.WriteLine("2. Просмотреть всех водителей");
            Console.WriteLine("3. Просмотреть все категории прав");
            Console.WriteLine("4. Просмотреть категории прав у водителя");
            Console.WriteLine("5. Просмотреть все маршруты");
            Console.WriteLine("6. Просмотреть все рейсы");
            Console.WriteLine("7. Добавить новый тип машины");
            Console.WriteLine("8. Добавить водителя");
            Console.WriteLine("9. Добавить категорию прав");
            Console.WriteLine("10. Добавить новый маршрут");
            Console.WriteLine("11. Добавить новый рейс");
            Console.WriteLine("0. Выход");
            
            string choice = Console.ReadLine();
            
            Console.ResetColor();
            Console.Clear();

            switch (choice)
            {
                case "1": DatabaseRequests.GetTypeCarQuery(); break;
                case "2": DatabaseRequests.GetDriverQuery(); break;
                case "3": DatabaseRequests.GetRightsCategoryQuery(); break;
                case "4": Commands.ViewRights(); break;
                case "5": DatabaseRequests.GetItineraryQuery(); break;
                case "6": DatabaseRequests.GetRouteQuery(); break;
                case "7": Commands.AddCarType(); break;
                case "8": Commands.AddDriver(); break;
                case "9": Commands.AddDriverRights(); break;
                case "10":Commands.AddItinerary(); break;
                case "11": Commands.AddRoute(); break;
                case "0":
                    return;
                default: Console.WriteLine("Неверный выбор. Попробуйте еще раз.");
                    break;
            }
        }
    }
}
public static class Commands
{
    //Функция просмотра категорий прав у водителя
    public static void ViewRights()
    {
        Console.Write("Введите ID водителя: ");
        try
        {
            int driver = int.Parse(Console.ReadLine());
            Console.WriteLine();
            DatabaseRequests.GetDriverRightsCategoryQuery(driver);
        }
        catch (FormatException)
        {
            Console.WriteLine("Ошибка. Неправильный формат ввода");
        }
    }
    //Функция добавления нового типа автомобиля
    public static void AddCarType()
    {
        Console.Write("Введите название нового типа автомобиля: ");
        DatabaseRequests.AddTypeCarQuery(Console.ReadLine());
    }
    //Функция добавления нового водителя
    public static void AddDriver()
    {
        Console.Write("Введите имя: ");
        string name = Console.ReadLine();
        Console.Write("Введите фамилию: ");
        string surname = Console.ReadLine();
        Console.Write("Введите дату рождения (YYYY.MM.DD): ");
        string birthday = Console.ReadLine();
        try
        {
            DatabaseRequests.AddDriverQuery(name, surname, DateTime.Parse(birthday));
        }
        catch (FormatException)
        {
            Console.WriteLine("Ошибка. Неправильный формат ввода");
        }
    }
    //Функция добавления категории прав водителю
    public static void AddDriverRights()
    {
        try
        {
            Console.Write("Введите ID водителя: ");
            int driver = int.Parse(Console.ReadLine());
            Console.Write("Введите ID категории: ");
            int rights = int.Parse(Console.ReadLine());
            DatabaseRequests.AddDriverRightsCategoryQuery(driver, rights);
            
            Console.Write("Введите название категории: "); 
            string name = Console.ReadLine();
            DatabaseRequests.AddRightsCategoryQuery(name);
        }
        catch (FormatException)
        {
            Console.WriteLine("Ошибка. Неправильный формат ввода");
        }
    }
    
    //Функция добавления маршрута
    public static void AddItinerary()
    {
        Console.Write("Введите название маршрута: ");
        string name = Console.ReadLine();
        DatabaseRequests.AddItineraryQuery(name);
    }
    //Функция добавления рейса
    public static void AddRoute()
    {
        try
        {
            Console.Write("Введите ID водителя: ");
            int driver = int.Parse(Console.ReadLine());
            Console.Write("Введите ID автомобиля: ");
            int car = int.Parse(Console.ReadLine());
            Console.Write("Введите ID маршрута: ");
            int itinerary = int.Parse(Console.ReadLine());
            Console.Write("Введите число пассажиров: ");
            int number = int.Parse(Console.ReadLine());
            
            DatabaseRequests.AddRouteQuery(driver, car, itinerary, number);    
        }
        catch (FormatException)
        {
            Console.WriteLine("Ошибка. Неправильный формат ввода");
        }
    }
}



public static class DatabaseRequests
{
    //Метод получения типов автомобилей из БД
    public static void GetTypeCarQuery()
    {
        var querySql = "SELECT * FROM type_car";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            Console.WriteLine($"| ID: {reader[0]} | Название: {reader[1]} |");
        }
    }
    
    //Метод получения всех водителей из БД
    public static void GetDriverQuery()
    {
        var querySql = "SELECT * FROM driver";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine($"| ID: {reader[0]} | Имя: {reader[1]} | Фамилия: {reader[2]} | Дата рождения: {reader[3]} |");
        }
    }
    
    //Метод получения всех категорий прав из БД
    public static void GetRightsCategoryQuery()
    {
        var querySql = "SELECT * FROM rights_category";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine($"| ID: {reader[0]} | Категория: {reader[1]}|");
        }
    }
    
    //Метод получения категорий прав у водителя из БД
    public static void GetDriverRightsCategoryQuery(int driver)
    {
        var querySql = "SELECT dr.first_name, dr.last_name, rc.name " +
                       "FROM driver_rights_category " +
                       "INNER JOIN driver dr on driver_rights_category.id_driver = dr.id " +
                       "INNER JOIN rights_category rc on rc.id = driver_rights_category.id_rights_category " +
                       $"WHERE dr.id = {driver};";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine($"| Имя: {reader[0]} | Фамилия: {reader[1]} | Категория прав: {reader[2]} |");
        }
    }
    
    //Метод получения всех автомобилей из БД
    public static void GetCarsQuery()
    {
        var querySql = "SELECT * FROM car";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine($"| ID: {reader[0]} | ID типа машины: {reader[1]} | Название: {reader[2]} | Номера: {reader[3]} | Вместимость пассажиров: {reader[4]} |");
        }
    }
    
    //Метод получения всех маршрутов из БД
    public static void GetItineraryQuery()
    {
        var querySql = "SELECT * FROM itinerary";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine($"| ID: {reader[0]} | Маршрут: {reader[1]} |");
        }
    }
    
    //Метод получения рейсов из БД
    public static void GetRouteQuery()
    {
        var querySql = "SELECT * FROM route";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine($"| ID: {reader[0]} | ID водителя: {reader[1]} | ID автомобиля: {reader[2]} | ID маршрута: {reader[3]} | Число пассажиров: {reader[4]} |");
        }
    }
    
    //Метод добавления нового типа автомобиля в БД
    public static void AddTypeCarQuery(string name)
    {
        var querySql = $"INSERT INTO type_car(name) VALUES ('{name}')";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        cmd.ExecuteNonQuery();
    }
    
    //Метод добавления нового водителя в БД
    public static void AddDriverQuery(string firstName, string lastName, DateTime birthdate)
    {
        var querySql = $"INSERT INTO driver(first_name, last_name, birthdate) VALUES ('{firstName}', '{lastName}', '{birthdate}')";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        cmd.ExecuteNonQuery();
    }
    
    //Метод добавления новой категории прав в БД
    public static void AddRightsCategoryQuery(string name)
    {
        var querySql = $"INSERT INTO rights_category(name) VALUES ('{name}')";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        cmd.ExecuteNonQuery();
    }
    
    //Метод добавления категории прав водителю в БД
    public static void AddDriverRightsCategoryQuery(int driver, int rightsCategory)
    {
        var querySql = $"INSERT INTO driver_rights_category(id_driver, id_rights_category) VALUES ({driver}, {rightsCategory})";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        cmd.ExecuteNonQuery();
    }
    
    //Метод добавления нового автомобиля в БД
    public static void AddCarsQuery(int id_type_car, string name, string state_number, int number_passengers)
    {
        var querySql = $"INSERT INTO car(id_type_car, name, state_number, number_passengers) VALUES ({id_type_car}, '{name}', '{state_number}', {number_passengers})";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        cmd.ExecuteNonQuery();
    }

    //Метод добавления нового маршрута в БД
    public static void AddItineraryQuery(string name)
    {
        var querySql = $"INSERT INTO itinerary(name) VALUES ('{name}')";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        cmd.ExecuteNonQuery();
    }

    //Метод добавления нового рейса в БД
    public static void AddRouteQuery(int id_driver, int id_car, int id_itinerary, int number_passengers)
    {
        var querySql = $"INSERT INTO route(id_driver, id_car, id_itinerary, number_passengers) VALUES ({id_driver}, {id_car}, {id_itinerary}, {number_passengers})";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        cmd.ExecuteNonQuery();
    }
}



public static class DatabaseService
{
    private static NpgsqlConnection? _connection;
    private static string GetConnectionString()
    {
        //Тут вводим данные от своей БД
return @"Host=10.30.0.137;Port=5432;Database=gr624_veoal;Username=gr624_veoal;Password=Qwert544";;
    }
    public static NpgsqlConnection GetSqlConnection()
    {
        if (_connection is null)
        {
            _connection = new NpgsqlConnection(GetConnectionString());
            _connection.Open();
        }
        
        return _connection;
    }
}