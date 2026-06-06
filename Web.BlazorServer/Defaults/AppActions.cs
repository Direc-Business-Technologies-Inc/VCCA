using System.ComponentModel;

namespace Web.BlazorServer.Defaults;

public enum AppActions
{
    #region System Actions
    [Description("Get Navigation Routes")]
    GetNavigationRoutes,
    #endregion System Actions

    #region Others
    [Description("Get Customers")]
    GetCustomers,
    [Description("Get Vendors")]
    GetVendors,
    [Description("Get Vendor")]
    GetVendor,
    [Description("Get Revenue Streams")]
    GetRevenueStreams,
    [Description("Get Delivery Types")]
    GetDeliveryTypes,
    [Description("Login")]
    Login,
    [Description("Logout")]
    Logout,
    #endregion Others

    #region Administration - Module Management
    [Description("Get All Modules")]
    GetAllModules,
    #endregion Administration - Module Management

    #region Administration - Authorization
    [Description("Get Role Permissions")]
    GetRolePermissions,
    [Description("Get User Permissions")]
    GetUserPermissions,
    [Description("Update User Permissions")]
    UpdateUserPermissions,
    [Description("Update Role Permissions")]
    UpdateRolePermissions,
    [Description("Cascade Role Permissions")]
    CascadeRolePermissions,
    #endregion Administration - Authorization

    #region Administration - User Management
    [Description("Get All Users")]
    GetAllUsers,
    [Description("Create User")]
    CreateUser,
    [Description("View User")]
    ViewUser,
    [Description("Update User")]
    UpdateUser,
    #endregion Administration - User Management

    #region Administration - Role Management
    [Description("Get All Roles")]
    GetAllRoles,
    [Description("Create Role")]
    CreateRole,
    [Description("View Role")]
    ViewRole,
    [Description("Update Role")]
    UpdateRole,
    #endregion Administration - Role Management

    #region Administration - Truck Assignment
    [Description("Get All Trucks")]
    GetAllTrucks,
    [Description("Get Truck")]
    GetTruck,
    [Description("View Truck")]
    ViewTruck,
    [Description("Update Truck")]
    UpdateTruck,
    #endregion Administration - Truck Assignment

    #region Transaction - Inventory
    [Description("Get All Tanks")]
    GetAllTanks,
    [Description("Get All Tank Statuses")]
    GetAllTankStatuses,
    [Description("Create Tank")]
    CreateTank,
    [Description("View Tank")]
    ViewTank,
    [Description("Update Tank")]
    UpdateTank,

    [Description("Get All Items")]
    GetAllItems,
    #endregion Transaction - Inventory

    #region Transaction - Procurement
    [Description("Get Next Order DocNum")]
    GetOrderDocNum,
    [Description("Get All Orders")]
    GetAllOrders,
    [Description("Create Order")]
    CreateOrder,
    [Description("View Order")]
    ViewOrder,
    [Description("Update Order")]
    UpdateOrder,
    #endregion Transaction - Procurement

    #region Transaction - InHouse Schedule
    [Description("Get Next InHouse Schedules DocNum")]
    GetInHouseSchedulesDocNum,
    [Description("Get All InHouse Scheduless")]
    GetAllInHouseSchedules,
    [Description("Get InHouse Schedule")]
    GetInHouseSchedule,
    [Description("Create InHouse Schedules")]
    CreateInHouseSchedules,
    [Description("View InHouse Schedules")]
    ViewInHouseSchedules,
    [Description("Update InHouse Schedules")]
    UpdateInHouseSchedules,
    #endregion Transaction - InHouse Schedule

    #region Transaction - ThirdParty Schedule
    [Description("Get Next Trip Ticket DocNum")]
    GetThirdPartySchedulesDocNum,
    [Description("Get All ThirdParty Scheduless")]
    GetAllThirdPartySchedules,
    [Description("Get ThirdParty Schedule")]
    GetThirdPartySchedule,
    [Description("Create ThirdParty Schedules")]
    CreateThirdPartySchedules,
    [Description("View ThirdParty Schedules")]
    ViewThirdPartySchedules,
    [Description("Update ThirdParty Schedules")]
    UpdateThirdPartySchedules,
    #endregion Transaction - ThirdParty Schedule

    #region Transaction - Trip Ticket
    [Description("Get Next Trip Ticket DocNum")]
    GetTripTicketDocNum,
    [Description("Get All Trip Tickets")]
    GetAllTripTickets,
    [Description("Get ThirdParty Schedule")]
    GetThirdParty,
    [Description("Create Trip Ticket")]
    CreateTripTicket,
    [Description("View Trip Ticket")]
    ViewTripTicket,
    [Description("Update Trip Ticket")]
    UpdateTripTicket,
    #endregion Transaction - Trip Ticket

    #region Transaction - PuchaseOrder
    [Description("Get All Purchase Orders")]
    GetAllPurchaseOrders,
    #endregion

}