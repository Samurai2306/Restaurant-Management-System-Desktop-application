namespace RestaurantSystem.Core.Enums;

/// <summary>
/// Enum representing the status of a table
/// </summary>
public enum TableStatus
{
    Available,
    Reserved,
    Occupied,
    Cleaning,
    OutOfService
}

/// <summary>
/// Enum representing the location of a table
/// </summary>
public enum TableLocation
{
    Window,
    Bar,
 MainHall,
    Terrace,
    VipRoom
}

/// <summary>
/// Enum representing the status of a reservation
/// </summary>
public enum ReservationStatus
{
    Pending,
    Confirmed,
    CheckedIn,
    Completed,
    Cancelled,
    NoShow
}

/// <summary>
/// Enum representing the status of an order
/// </summary>
public enum OrderStatus
{
    New,
InProgress,
    Ready,
    Served,
    Paid,
    Cancelled
}

/// <summary>
/// Enum representing the category of a dish
/// </summary>
public enum DishCategory
{
    Appetizer,
    Soup,
    Salad,
    MainCourse,
    SideDish,
 Dessert,
    Beverage,
    Alcohol
}

/// <summary>
/// Enum representing food allergens
/// </summary>
public enum Allergen
{
    None,
    Gluten,
    Lactose,
  Nuts,
    Eggs,
    Fish,
    Shellfish,
    Soy,
    Celery,
    Mustard,
    Sesame,
    Sulfites
}