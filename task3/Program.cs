using Org.BouncyCastle.Security;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace task3 {
    class Program {
        static RandomNumberGenerator secureRnd = new RNGCryptoServiceProvider();
        static Random random = new Random();

        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("There are no arguments");
                return;
            } else if (args.Length % 2 == 0 || args.Length < 3) {
                Console.WriteLine("An odd number of parameters greater than 1 is expected");
                return;
            } else if (args.ToList().Distinct().Count() != args.Length) {
                Console.WriteLine("The arguments must be unique");
                return;
            }

            while (Menu(args)) {
                Console.Write("Press any key");
                Console.ReadKey();
                Console.WriteLine("\n");
            }
        }

        static bool Menu(string[] moves) {
            var moveCount = moves.Length;
            var key = GenerateKey();
            var computerMove = random.Next(moveCount);
            var hmac = HMACHach(moves[computerMove], key);

            Console.WriteLine($"HMAC: {hmac}");
            Console.WriteLine($"Avaliable moves:");
            for (int i = 0; i < moveCount; i++) {
                Console.WriteLine($"{i + 1} - {moves[i]}");
            }
            Console.WriteLine($"0 - exit");

            int userMove = Enter(value => value >= 0 && value <= moveCount);

            if (userMove-- == 0) {
                return false;
            }

            Console.WriteLine($"Your move: {moves[userMove]}");
            Console.WriteLine($"Computer move: {moves[computerMove]}");

            Result(userMove, computerMove, moveCount);

            Console.WriteLine($"HMAC key: {key}");

            return true;
        }

        static string HMACHach(string str, string key) {
            var hmac = new HMACSHA256(Encoding.Default.GetBytes(key));
            byte[] bstr = Encoding.Default.GetBytes(str);
            var bhash = hmac.ComputeHash(bstr);

            return BitConverter.ToString(bhash).Replace("-", string.Empty);
        }

        static string GenerateKey() {
            var bkey = new byte[16];
            secureRnd.GetBytes(bkey);
            return BitConverter.ToString(bkey).Replace("-", string.Empty);
        }

        static int Enter(Func<int, bool> condition) {
            while (true) {
                try {
                    Console.Write("Enter your move: ");
                    int key = Convert.ToInt32(Console.ReadLine());

                    if (condition(key)) {
                        return key;
                    } else {
                        throw new ArgumentException();
                    }
                } catch (ArgumentException) {
                    Console.WriteLine("Invalide move");
                } catch (Exception) {
                    Console.WriteLine("Input error");
                }
            }
        }

        static void Result(int userMove, int computerMove, int moveCount) {
            var moveCountHalf = moveCount / 2;
            var lenBetweenMoves = Math.Abs(userMove - computerMove);
            
            if (userMove == computerMove) {
                Console.WriteLine("Draw!");
            } else if (computerMove < userMove && lenBetweenMoves <= moveCountHalf ||
                computerMove > userMove && lenBetweenMoves > moveCountHalf) {
                Console.WriteLine("You win!");
            } else {
                Console.WriteLine("You lose!");
            }
        }
    }
}
