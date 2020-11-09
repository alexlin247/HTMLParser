using System;
using System.Text;
using MySql.Data.MySqlClient;

namespace HTMLParser
{
    public static class HTMLParser
    {
        public static void Main()
        {
            //Test(@"test.txt", "output.json");
            TestAll();
        }

        public static void TestAll()
        {
            try
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();

                builder.Server = "localhost";
                builder.Port = 3306;
                builder.Database = "dccovidconnect";
                builder.UserID = "root";
                builder.Password = "password";
                using (MySqlConnection connection = new MySqlConnection(builder.ToString()))
                {
                    connection.Open();
                    string query = @"SELECT post_title, MAX(post_date_gmt) AS post_date, post_content
                                        FROM covidco_wp_posts
                                        WHERE NULLIF(post_content, '') IS NOT NULL AND NULLIF(post_title, '') IS NOT NULL
                                        GROUP BY post_title
                                        ORDER BY post_title ASC";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("Parsing {0}", reader.GetString(0));
                                Parser parser = new Parser(reader.GetString(2));
                                parser.Parse();
                                // Console.WriteLine(parser.Debug);
                                // Console.WriteLine(parser.Output);
                                System.IO.File.WriteAllText($"output/{reader.GetString(0)}.json", parser.Output);
                                Console.WriteLine("////////////////////////\n");

                            }
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Test(string path, string output)
        {
            string source = System.IO.File.ReadAllText(path);
            Parser parser = new Parser(source);
            parser.Parse();
            Console.WriteLine(parser.Output);
            System.IO.File.WriteAllText(output, parser.Output);
        }
    }
}