<h1>Online Parish Administration</h1>

A lightweight, CRM web application, designed for tracking church members & donations.  Admin accounts can manage member information, electronic payments, and track donations.  Members can access their own records online and view/update their information.
Designed to be extremely simple, the application can be tailored for use with any faith group, non-profit, clubs, etc.

Released under GNU General Public License v3.0

The Web.config file contains a few setting that should be populated before first use:

    <add key="admin:Email" value="" />
    <add key="smtp:Host" value="" />
    <add key="smtp:Port" value="" />
    <add key="smtp:Account" value="" />
    <add key="smtp:Password" value="" />

As is, the application will compile and run with a local database under App_Data.  On initial creation an admin-owner account will be created with a random password.  Use the Forgot Password link to reset the password for this account.

As is, the application is designed to work with DonorBox/Stripe & Square.  This will require updating the DonorBox links and setting the appropriate API keys:
    
    <add key="key:Square" value="" />
    <add key="key:Stripe" value="" />


