using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace IntegradorFinalHotel
{
    public class Inicio
    {

        struct ReservasStruct
        {
            public int IdReserva;
            public long dniHuesped;
            public int numeroHabitacion;
            public DateTime checkIn;
            public int cantidadNoches;

            public ReservasStruct(int IdReserva, long dniHuesped, int numeroHabitacion, DateTime checkIn, int cantidadNoches)
            {
                this.IdReserva = IdReserva;
                this.dniHuesped = dniHuesped;
                this.numeroHabitacion = numeroHabitacion;
                this.checkIn = checkIn;
                this.cantidadNoches = cantidadNoches;
            }
        }

        // Variables globales
        static int numeroHabitacion; // Almacena el número de la habitación seleccionada
        static int idReserva = 0; // Identificador de la reserva
        static Boolean salida = true; // Controla la salida del menú principal

        // Matrices para gestionar la disponibilidad de habitaciones para cada mes
        static bool[,] octubre = new bool[31, 10]; // 31 días x 10 habitaciones para octubre
        static bool[,] noviembre = new bool[30, 10]; // 30 días x 10 habitaciones para noviembre
        static bool[,] diciembre = new bool[31, 10]; // 31 días x 10 habitaciones para diciembre

        // Listas para almacenar información de huéspedes y reservas
        static List<(string NombreHuesped, int NumeroHabitacion, long dni, string mail)> huespedes = new List<(string, int, long, string)>();
        static List<ReservasStruct> reservas = new List<ReservasStruct>();

        static void Main(string[] args)
        {
       
        inicializarArreglos();
            // Bucle principal del menú
            do
            {
                salida = MenuPrincipal(menuInteractivo());
            } while (salida); // Repite mientras no se elija la opción de salida
        }

        // Método para mostrar el menú principal
        static Boolean MenuPrincipal(string eleccion)
        {
            Console.Clear();
            
            Boolean salidaMenu = true;
            //.ForegroundColor = ConsoleColor.White;
           // menuOpciones("Hotel Genesis",opciones);
           // string eleccion = Console.ReadLine();

            switch (eleccion)
            {
                case "1": agregarReserva(); break; // Llama al método para agregar una reserva
                                                   // case "2": modificarReserva(); break; // Para implementar la modificación de reservas
                                                    case "3": eliminarReserva(); break; // Para implementar la cancelación de reservas
                                                   // case "4": busquedaReserva(); break; // Para buscar reservas por nombre
                                                   // case "5": listarReservasOrdenadas(); break; // Para listar reservas ordenadas
                case "6": salidaMenu = false; return salidaMenu; // Salida del menú
                default:
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ingreso inválido");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("Presiona cualquier tecla para continuar...");
                    Console.ReadKey();
                    Console.Clear();
                    break;
            }
            return salidaMenu;
        }
        static string   menuInteractivo()
        {
            string[] opciones = new string[] {
             "Elija la opción:",
            "1. Crear Reserva",
            "2. Modificar Reserva",
            "3. Cancelar Reserva",
            "4. Buscar huesped por nombre",
           "5. Listar reservas ordenadas",
            "6. Salir" };
            // Índice de la opción actualmente seleccionada
            int seleccionActual = 0;

            // Variable para almacenar la tecla presionada
            ConsoleKey key;

            // Bucle principal que continúa hasta que se presione Enter
            do
            {
                // Limpia la consola en cada iteración para redibujar el menú
                Console.Clear();

                // Itera sobre las opciones del menú
                for (int i = 1; i < opciones.Length; i++)
                {
                    // Si la opción es la actualmente seleccionada, se resalta
                    if (i == seleccionActual)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow; // Cambia el color del texto a rojo
                        Console.WriteLine("* " + opciones[i]); // Imprime la opción con un marcador
                        Console.ResetColor(); // Restablece el color del texto
                    }
                    else
                    {
                        // Imprime las opciones que no están seleccionadas
                        Console.WriteLine("  " + opciones[i]);
                    }
                }

                // Captura la tecla presionada por el usuario sin mostrarla en la consola
                key = Console.ReadKey(true).Key;

                // Maneja la tecla presionada
                switch (key)
                {
                    // Si se presiona la flecha hacia arriba y no estamos en la primera opción
                    case ConsoleKey.UpArrow:
                        if (seleccionActual > 0)
                        {
                            seleccionActual--; // Mueve la selección una opción hacia arriba
                        }
                        break;

                    // Si se presiona la flecha hacia abajo y no estamos en la última opción
                    case ConsoleKey.DownArrow:
                        if (seleccionActual < opciones.Length - 1)
                        {
                            seleccionActual++; // Mueve la selección una opción hacia abajo
                        }
                        break;
                }
            } while (key != ConsoleKey.Enter); // El bucle continúa hasta que se presione Enter

            // Limpia la consola al finalizar el bucle
            Console.Clear();

            switch (seleccionActual)
            {
                

                    
                case 1:return "1" ; break;
                case 2: return "2" ; break;
                case 3:return "3" ; break;
                case 4: return "4" ; break;
                case 5: return "5"; break;

            }
            return "6";
        }
        static void menuOpciones(string titulo, string[] opciones)
        {
          
            int longitudMax=titulo.Length;
            foreach (string i in opciones)
            {
                if (i.Length > longitudMax)
                {
                    longitudMax = i.Length;
                }
            }
            Console.ForegroundColor= ConsoleColor.DarkGreen;
            string borde = new string('*', longitudMax + 4);
            Console.WriteLine(borde);                                
            Console.WriteLine($"* {CentrarTexto(titulo,longitudMax)} *"); 
            Console.WriteLine(borde);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            for (int i = 0; i < opciones.Length; i++)
            {
                if (i == 0)
                {
                    Console.WriteLine($"* {CentrarTexto(opciones[i], longitudMax)} *");
                }
                else
                {
                    Console.WriteLine($"* {opciones[i].PadRight(longitudMax)} *");
                }
            }
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(borde);
            Console.ResetColor();
        }//Funcion Para Dibujar Menu
        static string CentrarTexto(string texto, int longitudMaxima)
        {
            int espaciosTotales = longitudMaxima - texto.Length;   // Calcula los espacios que faltan para alinear
            int espaciosIzquierda = espaciosTotales / 2;           // Espacios a la izquierda
            int espaciosDerecha = espaciosTotales - espaciosIzquierda; // Espacios a la derecha

            // Usa PadRight para centrar el texto
            return new string(' ', espaciosIzquierda) + texto + new string(' ', espaciosDerecha).PadRight(espaciosDerecha);
        }//Funcion para centrar el texto en la funcion dibujar menu
        static void inicializarArreglos()
        {
       
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    octubre[i, j] = false;
                    diciembre[i, j] = false;
                }
            }
            for (int i = 0; i < 29; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    noviembre[i, j] = false;
                }
            }
        }
        static void agregarReserva()
        {///datos del huesped
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.Write("Ingrese el nombre del huésped: ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            string nombreHuesped = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Ingrese el DNI del huésped: ");
            bool valIngreso = true;
            long dni;
            do
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                string ingresoDni = Console.ReadLine();
               
                 valIngreso =long.TryParse(ingresoDni, out dni);
                if (!valIngreso)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ingreso invalido! vuelva a ingresar:");
                }
            } while (!valIngreso);//validacion de ingreso long
           
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Ingrese el mail del huésped: ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            string mail = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            ///datos de la reserva
            string[] opcionesMes = new string[] {
             "Elija la opción:",
            "1. Octubre",
            "2. Noviembre",
            "3. Diciembre"         
        };
            menuOpciones("Ingrese la fecha de check-In:", opcionesMes);
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            string opcion = Console.ReadLine();
            int mes;
            int dia;
            int cantNoches;
            bool disponibilidad = true;
            int habitacion;
            switch (opcion)
            {
                case "1":
                    Console.ForegroundColor = ConsoleColor.White;
                    mes = 10; // Corresponde a Octubre
                    Console.Write("Ingrese el día: ");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    dia = int.Parse(Console.ReadLine());
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Ingrese la cantidad de noches: ");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    cantNoches = int.Parse(Console.ReadLine());

                    do
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("Ingrese del 1 al 10 el numero de habitacion: ");
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        numeroHabitacion = int.Parse(Console.ReadLine()) - 1;
                        disponibilidad = verificarDisponibilidad(octubre, dia, cantNoches, numeroHabitacion);
                        if (disponibilidad)
                        {
                            idReserva++;
                            DateTime checkIn = new DateTime(2025, mes, dia);
                            ReservasStruct reserva = new ReservasStruct();
                            reservas.Add(reserva);
                        }
                        else
                        {

                            obtenerHabitacionesDisponibles(diciembre, dia, cantNoches);
                        }
                    } while (!disponibilidad);

                   // mostrarMatriz(octubre);
                    break;

                case "2":
                    Console.ForegroundColor = ConsoleColor.White;
                    mes = 11; // Corresponde a Noviembre
                    Console.Write("Ingrese el día: ");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    dia = int.Parse(Console.ReadLine());
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Ingrese la cantidad de noches: ");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    cantNoches = int.Parse(Console.ReadLine());
                    do
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("Ingrese del 1 al 10 el numero de habitacion: ");
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        numeroHabitacion = int.Parse(Console.ReadLine()) - 1;
                        disponibilidad = verificarDisponibilidad(octubre, dia, cantNoches, numeroHabitacion);
                        if (disponibilidad)
                        {
                            idReserva++;
                            DateTime checkIn = new DateTime(2025, mes, dia);
                            ReservasStruct reserva = new ReservasStruct();
                            reservas.Add(reserva);
                        }
                        else
                        {

                            obtenerHabitacionesDisponibles(diciembre, dia, cantNoches);
                        }
                    } while (!disponibilidad);
                    break;

                case "3":
                    Console.ForegroundColor = ConsoleColor.White;
                    mes = 12; // Corresponde a Diciembre
                    Console.Write("Ingrese el día: ");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    dia = int.Parse(Console.ReadLine());
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Ingrese la cantidad de noches: ");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    cantNoches = int.Parse(Console.ReadLine());

                    // Verifica disponibilidad para diciembre
                    do
                    {
                        Console.Write("Ingrese del 1 al 10 el numero de habitacion: ");
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                       numeroHabitacion = int.Parse(Console.ReadLine()) - 1;
                        disponibilidad = verificarDisponibilidad(octubre, dia, cantNoches,numeroHabitacion);
                        if (disponibilidad)
                        {
                            idReserva++;
                            DateTime checkIn = new DateTime(2025, mes, dia);
                            ReservasStruct reserva = new ReservasStruct();
                            reservas.Add(reserva);
                        }
                        else
                        {

                            obtenerHabitacionesDisponibles(diciembre, dia, cantNoches);
                        }
                    } while (!disponibilidad);

                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Opción inválida!");
                    break;
            }

            if (disponibilidad)
            {
               
                huespedes.Add((nombreHuesped, numeroHabitacion, dni, mail));
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Reserva creada con éxito!");
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
           
            Console.WriteLine("Apreta cualquier tecla para regresar al menu...");
            Console.ReadKey();
            Console.Clear();
        }
        // Método para verificar la disponibilidad de habitaciones
        static bool verificarDisponibilidad(bool[,] mes, int dia, int cantidadNoches, int habitacion)
        {
            // Se verifica la disponibilidad de la habitación en los días consecutivos
            for (int i = dia; i < dia + cantidadNoches; i++)
            {
                if (i >= mes.GetLength(0) || mes[i, habitacion]) // Verifica si se sale de los días del mes o si ya está ocupado
                {
                    return false; // Habitación no disponible

                }
                else
                {
                    for (int k = dia; k < dia + cantidadNoches; k++)
                    {
                        // Marca los días como ocupados
                        mes[i-1, habitacion] = true;
                    }
                }
            }
            // Habitación disponible
            return true;
        }
        // Función que retorna una lista de las habitaciones disponibles para un rango de días en un mes específico.
        static void obtenerHabitacionesDisponibles(bool[,] mes, int dia, int cantidadNoches)
        {
            List<int> habitacionesLibres = new List<int>();
            Console.ForegroundColor= ConsoleColor.DarkRed;
            Console.WriteLine("Habitacion no disponible..!");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Verificando disponibilidad desde el día {dia} por {cantidadNoches} noches.");

            for (int i = dia; i <= dia+cantidadNoches  || i < mes.GetLength(1); i++)
            {
                if (!mes[dia, i])
                {
                    habitacionesLibres.Add(i);
                }
            }
               
            

            // Muestra las habitaciones disponibles.
            if (habitacionesLibres.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Habitaciones disponibles en el rango solicitado:");
                foreach (int habitacion in habitacionesLibres)
                {
                    Console.WriteLine($"Habitación {habitacion + 1}");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No hay habitaciones disponibles para el rango de fechas seleccionado.");
            }

            Console.ResetColor(); // Restaurar colores de consola.
        }

        static void eliminarReserva()
        {
            Console.Clear();
            long dniHues;
            int codigoEliminacion;
            bool valCodigo = true;
            bool eliminado = false;
            bool huespedEliminado = false; // Nueva bandera para el huésped
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("******_Eliminacion de una Reserva/Huesped_******");
            bool hayReservas = mostrarReservas();
            Console.ResetColor();
           

            if (hayReservas)
            {
                do
                {
                    Console.Write("Ingrese el id de la reserva que quiera eliminar: ");
                    Console.ResetColor();
                    string codigo = Console.ReadLine();
                    valCodigo = int.TryParse(codigo, out codigoEliminacion);
                    if (!valCodigo)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Ingrese formato que corresponda!");
                    }

                    for (int i = reservas.Count - 1; i >= 0; i--)
                    {
                        if (reservas[i].IdReserva == codigoEliminacion)
                        {
                            dniHues = reservas[i].dniHuesped;
                            reservas.RemoveAt(i);
                            eliminado = true;

                            for (int j = huespedes.Count - 1; j >= 0; j--)
                            {
                                if (huespedes[j].dni == dniHues)
                                {
                                    huespedes.RemoveAt(j);
                                    huespedEliminado = true; // Indicamos que se eliminó al huésped
                                }
                            }

                            if (!huespedEliminado)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("No se encontró un huésped con ese DNI.");
                            }
                        }
                    }

                    if (eliminado)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("¡Reserva eliminada!");

                        if (huespedEliminado)
                        {
                            Console.WriteLine("¡Huésped eliminado!");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("No se encontró un huésped para la reserva eliminada.");
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No se encontró la reserva.");
                    }

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("Apriete cualquier tecla para continuar...");
                    Console.ReadKey();
                } while (!valCodigo);
            }



            else if (!hayReservas)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("No existen registros");
                Console.ReadKey();
            }







        }
        static bool mostrarReservas()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            for (int i = 0; i < reservas.Count; i++)
            {
                Console.WriteLine($"Id:{reservas[i].IdReserva} - Dni del Huesped: {reservas[i].dniHuesped} - Numero de habitacion: {reservas[i].numeroHabitacion} - Check-in:  {reservas[i].checkIn.Day}/{reservas[i].checkIn.Month}/{reservas[i].checkIn.Year} -  Cantidad de noches: {reservas[i].cantidadNoches}");
                Console.WriteLine("--------------------------------------------------------");
            }
            if (reservas.Count == 0)
            {
                return false;
            }
            return true;

        }
        
        static void modificarReserva()
        {
            
        }
        static void buscarHuespedNombre()
        {

        }
        static void mostrarReservasOrdenadas()
        {


        }
        
    
    }





    }
















