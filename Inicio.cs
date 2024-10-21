﻿//Nuevos cambios 20/10
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
            public int IdReserva, NumeroHabitacion, CantidadNoches;
            public long DniHuesped;
            public DateTime CheckIn;

            public ReservasStruct(int idReserva, long dniHuesped, int numeroHabitacion, DateTime checkIn, int cantidadNoches)
            {
                IdReserva = idReserva;
                DniHuesped = dniHuesped;
                NumeroHabitacion = numeroHabitacion;
                CheckIn = checkIn;
                CantidadNoches = cantidadNoches;
            }
        }

        // Variables globales
        static int numeroHabitacion; // Almacena el número de la habitación seleccionada
        static int idReserva = 0;
        static Boolean salida = true; // Controla la salida del menú principal

        // Matrices para gestionar la disponibilidad de habitaciones para cada mes
        static bool[,] octubre = new bool[31, 10]; // 31 días x 10 habitaciones para octubre
        static bool[,] noviembre = new bool[30, 10];
        static bool[,] diciembre = new bool[31, 10];


        // Listas para almacenar información de huéspedes y reservas
        static List<(string NombreHuesped, int NumeroHabitacion, long dni, string mail)> huespedes = new List<(string, int, long, string)>();
        static List<ReservasStruct> reservas = new List<ReservasStruct>();

        static void Main(string[] args)
        {
            inicializarMatrices();
            // Bucle principal del menú
            do
            {
                salida = MenuPrincipal();
            } while (salida); // Repite mientras no se elija la opción de salida
        }
        static Boolean MenuPrincipal()
        {
            Console.Clear();
            string[] opciones = new string[] {
            "Elija la opción:",
            "1. Crear Reserva",
            "2. Modificar Reserva",
            "3. Cancelar Reserva",
            "4. Buscar huesped por nombre",
            "5. Listar reservas ordenadas",
            "6. Salir"
        };
            Boolean salidaMenu = true;
            Console.ForegroundColor = ConsoleColor.White;
            menuOpciones("Hotel Genesis", opciones);
            string eleccion = Console.ReadLine();

            switch (eleccion)
            {
                case "1":
                    agregarReserva();
                    break;
                // case "2": modificarReserva(); break; // Para implementar la modificación de reservas
                case "3":
                    eliminarReserva();
                    break;
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

        //Funcion para centrar texto del menu
        static string CentrarTexto(string texto, int longitudMaxima)
        {
            int espaciosTotales = longitudMaxima - texto.Length;   // Calcula los espacios que faltan para alinear
            int espaciosIzquierda = espaciosTotales / 2;           // Espacios a la izquierda
            int espaciosDerecha = espaciosTotales - espaciosIzquierda; // Espacios a la derecha

            // Usa PadRight para centrar el texto
            return new string(' ', espaciosIzquierda) + texto + new string(' ', espaciosDerecha).PadRight(espaciosDerecha);
        }

        //Inicializa boleano de matrices de meses con sus habitaciones todas en false
        static void inicializarMatrices()
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

            long dni;
            bool validacionIngresoDni, opcionValida, disponibilidad = true;
            string[] opcionesMes = new string[] {
             "Elija la opción:",
             "1. Octubre",
             "2. Noviembre",
             "3. Diciembre"
             };
            int mes, dia, cantNoches, opcionNumero;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.Write("Ingrese el nombre del huésped: ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            string nombreHuesped = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Ingrese el DNI del huésped: ");
            do
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                string ingresoDni = Console.ReadLine();
                validacionIngresoDni = long.TryParse(ingresoDni, out dni);
                if (!validacionIngresoDni)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ingreso invalido! vuelva a ingresar:");
                }
            } while (!validacionIngresoDni);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Ingrese el mail del huésped: ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            string mail = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;

            // valida que se ingrese bien la opcion del mes
            do
            {
                menuOpciones("Elije el Mes: ", opcionesMes);
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                string opcion = Console.ReadLine();

                opcionValida = int.TryParse(opcion, out opcionNumero);
                if (!opcionValida || opcionNumero > 3 || opcionNumero <= 0)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Opcion erronea! Vuelva a ingresar");
                    opcionValida = false;
                }
            } while (!opcionValida);

            //se elije el mes y se agrega a la matriz correspondiente
            switch (opcionNumero)
            {
                case 1:
                    mes = 10; // Corresponde a Octubre
                    Console.ForegroundColor = ConsoleColor.White;
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

                case 2:
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

                case 3:
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

        //Funcion Para Dibujar los asteriscos en el menu
        static void menuOpciones(string titulo, string[] opciones)
        {

            int longitudMax = titulo.Length;
            foreach (string i in opciones)
            {
                if (i.Length > longitudMax)
                {
                    longitudMax = i.Length;
                }
            }
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            string borde = new string('*', longitudMax + 4);
            Console.WriteLine(borde);
            Console.WriteLine($"* {CentrarTexto(titulo, longitudMax)} *");
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
                        mes[i - 1, habitacion] = true; //Marca los dias ocupados
                    }
                }
            }
            return true; //Habitacion disponible
        }
        // Función que retorna una lista de las habitaciones disponibles para un rango de días en un mes específico.
        static void obtenerHabitacionesDisponibles(bool[,] mes, int dia, int cantidadNoches)
        {
            List<int> habitacionesLibres = new List<int>();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Habitacion no disponible..!");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Verificando disponibilidad desde el día {dia} por {cantidadNoches} noches.");

            for (int i = dia; i <= dia + cantidadNoches || i < mes.GetLength(1); i++)
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
            bool valCodigo, eliminado = false, huespedEliminado = false;
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
                            dniHues = reservas[i].DniHuesped;
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

            else //Si no hay reservas
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
                Console.Write($"Id:{reservas[i].IdReserva} - Dni del Huesped: {reservas[i].DniHuesped}    ");
                Console.Write($"- Numero de habitacion: {reservas[i].NumeroHabitacion} -");
                Console.Write($" Check-in:  {reservas[i].CheckIn.Day}/{reservas[i].CheckIn.Month}/{reservas[i].CheckIn.Year} -");
                Console.WriteLine($" Cantidad de noches: {reservas[i].CantidadNoches}");
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





























