﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
            public long DniHuesped;
            public int NumeroHabitacion;
            public DateTime CheckIn;
            public int CantidadNoches;

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
       
        static string diaV;
        static string nombreHuesped;
        static string ingresoNoches = "";
        static string mail;
        static int habitacion;
        static int dia = 0;
        static long dni;
        static int numeroHabitacion; // Almacena el número de la habitación seleccionada
        static int idReserva = 0; // Identificador de la reserva
        static int cantNoches = 0;
        static bool disponibilidad = true;
        static bool validarD = true;
        static bool valNoche = true;
        static bool valHabitacion = true;
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
                    agregarReserva(); break; // Llama al método para agregar una reserva
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


        static string CentrarTexto(string texto, int longitudMaxima)
        {
            int espaciosTotales = longitudMaxima - texto.Length;   // Calcula los espacios que faltan para alinear
            int espaciosIzquierda = espaciosTotales / 2;           // Espacios a la izquierda
            int espaciosDerecha = espaciosTotales - espaciosIzquierda; // Espacios a la derecha

            // Usa PadRight para centrar el texto
            return new string(' ', espaciosIzquierda) + texto + new string(' ', espaciosDerecha).PadRight(espaciosDerecha);
        }//Funcion para centrar el texto en la funcion dibujar menu
        //Funcion para centrar el texto en la funcion dibujar menu
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
             nombreHuesped = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Ingrese el DNI del huésped: ");
            bool valIngreso = true;
            
            do
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                string ingresoDni = Console.ReadLine();

                valIngreso = long.TryParse(ingresoDni, out dni);
                if (!valIngreso)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ingreso invalido! vuelva a ingresar:");
                }
            } while (!valIngreso);//validacion de ingreso long (DNI)

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Ingrese el mail del huésped: ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            mail = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            ///datos de la reserva
            string[] opcionesMes = new string[] {
             "Elija la opción:",
            "1. Octubre",
            "2. Noviembre",
            "3. Diciembre"
        };
            bool opcionValida = true;
            int opcionNumero;
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
                    segunMes(octubre, 1, 31,10); // Ejecuta segunMes para cuando se elija octubre
                    break; // Sale del switch después de ejecutar el caso

                case 2:
                    segunMes(noviembre, 2, 30,11); // Ejecuta segunMes para cuando se elija noviembre
                    break; // Sale del switch después de ejecutar el caso

                case 3:
                    segunMes(diciembre, 3, 31,12); // Ejecuta segunMes para cuando se elija diciembre
                    break; // Sale del switch después de ejecutar el caso

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
        static void segunMes(bool[,] mesSelect, int numeroMes,int catDiasMes,int mes)
        {
           
            Console.ForegroundColor = ConsoleColor.White;
           
            Console.Write("Ingrese el día: ");

            do
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                diaV = Console.ReadLine();
                validarD = validarDia(diaV, numeroMes);

                if (!validarD)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ingreso no permitido!, vuelva a intentar");
                    Console.ResetColor();
                }
                if (validarD)
                {
                    dia = int.Parse(diaV);
                }

            } while (!validarD);



            Console.ForegroundColor = ConsoleColor.White;

            // validacion de noches:                    


            do
            {
                Console.Write("Ingrese la cantidad de noches: ");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                ingresoNoches = Console.ReadLine();
                valNoche = validarNoche(ingresoNoches,catDiasMes , dia);
                if (!valNoche)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ingreso no permitido!, vuelva a intentar");
                    Console.ResetColor();
                }


                if (valNoche)
                {
                    cantNoches = int.Parse(ingresoNoches);
                }

            } while (!valNoche);


            do
            {
                //validar numero de habitacion
                do
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Ingrese del 1 al 10 el numero de habitacion: ");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    string entradaHabitacion = Console.ReadLine();
                    valHabitacion = validarHabitacion(entradaHabitacion);
                    if (!valHabitacion)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Ingreso no permitido!, vuelva a intentar");
                        Console.ResetColor();
                    }


                    if (valHabitacion)
                    {
                        numeroHabitacion = int.Parse(entradaHabitacion);
                    }

                } while (!valHabitacion);


                disponibilidad = verificarDisponibilidad(mesSelect, dia, cantNoches, numeroHabitacion);
                if (disponibilidad)
                {
                    idReserva++;
                    DateTime checkIn = new DateTime(2025, mes, dia);
                    ReservasStruct reserva = new ReservasStruct(idReserva, dni, numeroHabitacion, checkIn, cantNoches);
                    reservas.Add(reserva);
                    huespedes.Add((nombreHuesped,numeroHabitacion,dni,mail));
                }
                else
                {

                    obtenerHabitacionesDisponibles(mesSelect, dia, cantNoches);
                }
            } while (!disponibilidad);
        }
        static bool validarNoche(string noche,int max,int dia)
        {
            int numeroNoches;
            bool validacionIngreso = int.TryParse(noche, out numeroNoches);
            max = max - dia;
            if (validacionIngreso)
            {
                if (max-numeroNoches <0 || numeroNoches>max || numeroNoches<0) {
                    validacionIngreso= false;
                }
               
            }
            return validacionIngreso;
        }
      static bool validarHabitacion(string ingresoHabitacion)
        {
            int numeroHabitacion=0;
            bool valHabitacion= int.TryParse(ingresoHabitacion,out numeroHabitacion);
            if (valHabitacion) { 
            if (numeroHabitacion<0 || numeroHabitacion > 10)
                {
                    valHabitacion=false;
                }
            
            }
            return valHabitacion;
            
        }
        static bool validarDia(string dia, int mes)
        {
            bool valDia = true;
            int numDia;
            valDia = int.TryParse(dia, out numDia);
            if (valDia)
            {
                valDia = true;
            }
            if (valDia)
            {
                switch (mes)
                {
                    case 1:
                        if (numDia <= 30 && numDia > 0)
                        {
                            valDia = true;
                        }
                        else
                        {
                            valDia = false;
                        }

                        break;
                    case 2:
                        if (numDia <= 31 && numDia > 0)
                        {
                            valDia = true;
                        }
                        else
                        {
                            valDia = false;
                        }

                        break;
                    case 3:
                        if (numDia <= 30 && numDia > 0)
                        {
                            valDia = true;
                        }
                        else
                        {
                            valDia=false;
                        }
                      
                        break;

                    default: return false;

                }
                
            }
            return valDia;

        }

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
        }//Funcion Para Dibujar Menu

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
                Console.Write($"Id:{reservas[i].IdReserva}");
                Console.Write($" - Dni del Huesped: {reservas[i].DniHuesped}");
                Console.Write($"- Numero de habitacion: {reservas[i].NumeroHabitacion}");
                Console.Write($"- Check-in:  {reservas[i].CheckIn.Day}/{reservas[i].CheckIn.Month}/{reservas[i].CheckIn.Year}");
                Console.Write($" -  Cantidad de noches: {reservas[i].CantidadNoches}");              
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
















