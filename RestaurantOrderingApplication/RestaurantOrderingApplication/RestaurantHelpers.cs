using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;

namespace RestaurantOrderingApplication
{
    public class RestaurantHelpers
    {
        public string[] AddOns;
        public static List<OrderDetails> AllOrderDetails = new List<OrderDetails>();
        public RestaurantHelpers()
        {
            AddOns = new string[] { "Mixed Vegetables", "Chicken", "Beef", "Spicy Suace", "Sweet Saucce", "Sour Cream", "Guacamole " };
        }

        public void Start(string ErrorMsg = "")
        {

            Console.Clear();
            if (!string.IsNullOrWhiteSpace(ErrorMsg))
            {
                PrintErrorMsg(ErrorMsg);
            }
            Console.WriteLine("=================================================");
            Console.WriteLine("Welcome to our restaurant");
            Console.WriteLine("=================================================\n");
            Console.WriteLine("Press 1 ==> Take Order");
            Console.WriteLine("Press 2 ==> View Reports");
            ConsoleKeyInfo Step1Inp = Console.ReadKey();
            Console.WriteLine("\n_________________________________________________\n");
            switch (Step1Inp.KeyChar.ToString())
            {
                case "1":
                    TakeOrders();
                    break;
                case "2":
                    ViewReports();
                    break;
                default:
                    //PrintErrorMsg();
                    Start("Please Enter one of the given options.");
                    break;
            }
        }

        #region ::: Methods to print :::
        // A reusable method to changes the colour of the the text and print the error message and then reset the changed color
        public void PrintErrorMsg(string Error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Error);
            Console.ResetColor();
        }

        // A reusable method to display the Orders so far placed/recorded.
        public void DisplayOrder(RiceBowl RiceBowlOrder)
        {
            Console.WriteLine("_________________________________________________");
            Console.WriteLine("Order:");
            Console.WriteLine("Rice Type: " + (RiceBowlOrder.RiceType == 1 ? "White Rice" : "Brown Rice"));
            // The ordered customizations/additionals are stored according to their indexes (array_index + 1).
            // Print the names of the orders Addons accordingly.
            foreach (string item in RiceBowlOrder.Additionals.Split(',', StringSplitOptions.RemoveEmptyEntries))
                Console.WriteLine(AddOns[int.Parse(item) - 1]);
            Console.WriteLine("_________________________________________________");
        }

        #endregion

        #region ::: Methods Used to Get Inputs From the User :::

        public int RiceBowl()
        {
            int RiceInput = 0;
            bool IsError = false;
            // Repeat this method untill user enters a valid input
            while (true)
            {
                Console.Clear();
                if (IsError)
                {
                    PrintErrorMsg("Please Enter one of the given options.");
                    IsError = false;
                }
                Console.WriteLine("Press 1 ==> White Rice");
                Console.WriteLine("Press 2 ==> Brown Rice");
                Console.WriteLine("Press 0 ==> Go Back");
                ConsoleKeyInfo RiceInp = Console.ReadKey();
                Console.WriteLine("\n_________________________________________________");

                int.TryParse(RiceInp.KeyChar.ToString(), out RiceInput);
                // Warn the user in order to make sure he doesn't cancel the order accidentally.
                if (RiceInp.KeyChar.ToString() == "0")
                {
                    int WarningVal = WarningFunc();
                    if (WarningVal == 1)
                        return 0;
                }
                // Return Either Brown Rice or White Rice based on the Input.
                else if (RiceInput < 3)
                    return RiceInput;
                else
                    IsError = true;
            }
        }

        public RiceBowl GetOrDeleteAddOnsOrder(RiceBowl RiceBowlOrder, bool IsAdd)
        {
            bool PrintError = false;
            string Error = "";
            // Repeat this method untill user enters a valid input
            while (true)
            {
                Console.Clear();
                if (PrintError)
                {
                    PrintErrorMsg(Error);
                    PrintError = false;
                }
                DisplayOrder(RiceBowlOrder);

                /*Assumption: Customizations are not tightly bound, 
                 for Ex: the customer can choose to add both the meat or no meat at all. 
                 Similarly can choose to have no sauce at all*/

                Console.WriteLine("Customizations:\n");

                // Display the available Customizations. 
                for (int i = 0; i < AddOns.Length; i++)
                {
                    Console.WriteLine("Press " + (i + 1) + " ==> " + "To Add " + AddOns[i]);
                }
                Console.WriteLine("\nKeep Entering the Customizations.\n\nOR ");
                Console.WriteLine("\nPress 9 ==> To Change Rice Type");
                Console.WriteLine("Press 0 ==> To Cancel the Order");
                Console.WriteLine("Press \"Enter\" ==> To Continue to Place Order.");
                ConsoleKeyInfo CustomizeInp = Console.ReadKey();
                Console.WriteLine("\n_________________________________________________\n");

                int AddOnsInp = 0;
                int.TryParse(CustomizeInp.KeyChar.ToString(), out AddOnsInp);

                if (CustomizeInp.Key == ConsoleKey.Enter)
                    return RiceBowlOrder;
                // Warn the user in order to make sure he doesn't cancel the order accidentally.
                else if (CustomizeInp.KeyChar.ToString() == "0")
                {
                    if (WarningFunc() == 1)
                        return null;
                }
                else if (AddOnsInp > 0 && AddOnsInp < 8)
                {
                    // A single method that Adds items to the order, or deletes item from it based on the situation (an argument)
                    /* Stores the Additionals as a string that contains the indexes(array_index+1) of the ordered stuff
                     separated by comma */
                    if (IsAdd)
                    {
                        // Warn if ordered item is already in the order.
                        if (RiceBowlOrder.Additionals.Contains(AddOnsInp.ToString()))
                        {
                            PrintError = true;
                            Error = "Entered Item already exists. Please select other options or Proceed to Checkout";
                        }
                        else
                            RiceBowlOrder.Additionals += (AddOnsInp + ",");
                    }
                    else
                    {
                        // Warn if the item to be deleted is not ordered.
                        if (!RiceBowlOrder.Additionals.Contains(AddOnsInp.ToString()))
                        {
                            PrintError = true;
                            Error = "Entered Item does not exist exists. Please select other options or Proceed to Checkout";
                        }
                        else
                            RiceBowlOrder.Additionals = RiceBowlOrder.Additionals.Replace((AddOnsInp + ","), "");
                    }
                }
                // Change the Rice type.
                else if (AddOnsInp == 9)
                    RiceBowlOrder.RiceType = RiceBowlOrder.RiceType == 1 ? 2 : 1;
                else
                {
                    PrintError = true;
                    Error = "Please Enter one of the given options.";
                }
            }
        }

        public int WarningFunc()
        {
            bool PrintMsg = false;
            // Repeat this method untill user enters a valid input
            while (true)
            {
                Console.Clear();
                if (PrintMsg)
                {
                    PrintErrorMsg("Please Enter one of the given options.");
                    PrintMsg = false;
                }
                Console.WriteLine("Are You Sure, you want to Cancel the order? ");
                Console.WriteLine("Press 1 ==> Yes");
                Console.WriteLine("Press 2 ==> No");
                ConsoleKeyInfo WarningInp = Console.ReadKey();
                Console.WriteLine("\n_________________________________________________\n");
                if (WarningInp.KeyChar.ToString() == "1" || WarningInp.KeyChar.ToString() == "2")
                    return int.Parse(WarningInp.KeyChar.ToString());
                else
                    PrintMsg = true;
            }
        }

        public int PlaceOrder(RiceBowl RiceBowlOrder)
        {
            bool PrintError = false;
            // Repeat this method untill user enters a valid input
            while (true)
            {
                Console.Clear();
                if (PrintError)
                {
                    PrintErrorMsg("Please Enter one of the given options.");
                    PrintError = false;
                }
                DisplayOrder(RiceBowlOrder);
                Console.WriteLine("Press \"Enter\" ==> Place Order");
                Console.WriteLine("Press 1 ==> Modify Order");
                Console.WriteLine("Press 0 ==> Cancel Order");
                ConsoleKeyInfo PlaceOrderInp = Console.ReadKey();
                Console.WriteLine("\n_________________________________________________");
                int OrderInp = 0;
                int.TryParse(PlaceOrderInp.KeyChar.ToString(), out OrderInp);
                if (PlaceOrderInp.KeyChar.ToString() == "0")
                {
                    // Warn the user in order to make sure he doesn't cancel the order accidentally.
                    if (WarningFunc() == 1)
                        return 0;
                }
                //Return 1 => to Modify Order, 2 => to place Order.
                else if (OrderInp == 1 || PlaceOrderInp.Key == ConsoleKey.Enter)
                    return OrderInp == 0 ? 2 : 1;
                else
                    PrintError = true;
            }
        }

        public RiceBowl ModifyOrder(RiceBowl RiceBowlOrder)
        {
            bool PrintError = false;
            // Repeat this method untill user enters a valid input
            while (true)
            {
                Console.Clear();
                if (PrintError)
                {
                    PrintErrorMsg("Please Enter one of the given options.");
                    PrintError = false;
                }
                DisplayOrder(RiceBowlOrder);
                // Print all the type of modifications available. 
                Console.WriteLine("Press \"Enter\" ==> To proceed to place order");
                Console.WriteLine("Press 1 ==> Add new items to the order");
                Console.WriteLine("Press 2 ==> Delete items from the order");
                Console.WriteLine("Press 9 ==> Change Rice Type");
                Console.WriteLine("Press 0 ==> Cancel the Order");

                ConsoleKeyInfo ModifyOrderInp = Console.ReadKey();
                Console.WriteLine("\n_________________________________________________");

                if (ModifyOrderInp.Key == ConsoleKey.Enter)
                    return RiceBowlOrder;
                else if (ModifyOrderInp.KeyChar.ToString() == "0")
                {
                    // Warn the user in order to make sure he doesn't cancel the order accidentally.
                    if (WarningFunc() == 1)
                        return null;
                }
                else
                {
                    switch (ModifyOrderInp.KeyChar.ToString())
                    {
                        // Call the GetOrDeleteAddOnsOrder method while setting is Argument IsAdd to true => add new items to the order.
                        // IsAdd to false => delete existing items from the order.
                        case "1":
                            RiceBowlOrder = GetOrDeleteAddOnsOrder(RiceBowlOrder, true);
                            break;
                        case "2":
                            RiceBowlOrder = GetOrDeleteAddOnsOrder(RiceBowlOrder, false);
                            break;
                        case "9":
                            RiceBowlOrder.RiceType = RiceBowlOrder.RiceType == 1 ? 2 : 1;
                            break;
                        default:
                            PrintError = true;
                            break;
                    }
                }
                //If the Order has been cancelled while Modifying thecustomizations.
                if (RiceBowlOrder == null)
                    return null;
            }
        }

        #endregion

        public void TakeOrders()
        {
            // Take the input on the type of rice required in the rice bowl.
            RiceBowl RiceBowlOrder = new RiceBowl();
            Console.WriteLine("Taking Orders");
            int RiceBowlInp = RiceBowl();
            if (RiceBowlInp == 0)
                Start();
            else
            {
                RiceBowlOrder.RiceType = RiceBowlInp;
                RiceBowlOrder.Additionals = string.Empty;

                // Take the input on all the Additiionals/Customizations required in the rice bowl.
                RiceBowlOrder = GetOrDeleteAddOnsOrder(RiceBowlOrder, true);
                // GetOrDeleteAddOnsOrder returns null if the user wants to cancel the order.
                if (RiceBowlOrder == null)
                    Start();
                else
                {
                    bool IsModyfying = true;
                    while (IsModyfying)
                    {
                        // GetOrDeleteAddOnsOrder returns null if the user wants to cancel the order.(While Modifying Customizations)
                        if (RiceBowlOrder == null)
                        {
                            IsModyfying = false;
                            Console.WriteLine("Order Cancelled!!!");
                        }
                        else
                        {
                            int PlaceOrderInp = PlaceOrder(RiceBowlOrder);
                            Console.Clear();
                            switch (PlaceOrderInp)
                            {
                                case 0:
                                    IsModyfying = false;
                                    Console.WriteLine("Order Cancelled!!!");
                                    break;
                                case 1:
                                    RiceBowlOrder = ModifyOrder(RiceBowlOrder);
                                    break;
                                default:
                                    IsModyfying = false;
                                    Console.WriteLine("Order Placed!!!");
                                    DisplayOrder(RiceBowlOrder);
                                    // Add the Placed Orders to a List,  which can be later used in Displaying the Records.
                                    AllOrderDetails.Add(new OrderDetails { RiceType = RiceBowlOrder.RiceType, Additionals = RiceBowlOrder.Additionals, OrderPlacedDateTime = DateTime.Now });
                                    break;
                            }
                        }
                    }
                    // Delay for 2 seconds before going ahead and taking the next order.
                    int milliseconds = 2000;
                    Thread.Sleep(milliseconds);
                    Start();
                }
            }
        }

        public void ViewReports()
        {
            Console.Clear();
            Console.WriteLine("================================================================");
            Console.WriteLine("Statistics");
            Console.WriteLine("================================================================\n");
            // Get the List of all order plaaced in the Last '1' Hour
            List<OrderDetails> ReportDetails = AllOrderDetails.Where(e => e.OrderPlacedDateTime >= DateTime.Now.AddHours(-1)).Select(e => e).ToList();
            // Calculate the counts. 
            int CountRiceBowlBeef = ReportDetails.Where(e => e.Additionals.Contains("3")).Count();
            int CountRiceBowlChicken = ReportDetails.Where(e => e.Additionals.Contains("2")).Count();
            int CountRiceBowlMixVeg = ReportDetails.Where(e => e.Additionals.Contains("1")).Count();
            int CountWhiteRiceBowl = ReportDetails.Where(e => e.RiceType == 1).Count();

            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("Total Number of Rice bowls ordered: " + ReportDetails.Count);
            Console.WriteLine("----------------------------------------------------------------\n");
            Console.WriteLine("Total Number of Rice bowls with White Rice ordered: " + CountWhiteRiceBowl);
            Console.WriteLine("Total Number of Rice bowls with Brown Rice ordered: " + (ReportDetails.Count - CountWhiteRiceBowl));
            Console.WriteLine("----------------------------------------------------------------\n");
            Console.WriteLine("Total Number of Rice bowls with Chciken ordered: " + CountRiceBowlChicken);
            Console.WriteLine("Total Number of Rice bowls with Beef ordered: " + CountRiceBowlBeef);
            Console.WriteLine("Total Number of Rice bowls with Mixed Vegetables ordered: " + CountRiceBowlMixVeg);
            Console.WriteLine("----------------------------------------------------------------");
            // Go back to start method to Take the next order or View reports once again.
            Console.WriteLine("Press Any Key to go Back");
            Console.ReadKey();
            Start();
        }
    }
    #region ::: User Defined Data Classes/Structures
    public class RiceBowl
    {
        public int RiceType { get; set; } // 1 ==> White Rice, 2 ==> Brown Rice
        public string Additionals { get; set; }
    }
    public class OrderDetails : RiceBowl
    {
        public DateTime OrderPlacedDateTime { get; set; }
    }
    #endregion
}
