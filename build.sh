#!/bin/bash
SOURCEROOT=/c/Users/Anders/source/repos/nopCommerce-Omnishop/src
PUBLISHROOT=/c/Users/Anders/source/repos/nopCommerce-Omnishop/published

echo "-------------------------------------------------------"
echo "Restoring packages for solution..."
echo "-------------------------------------------------------"
cd $SOURCEROOT
dotnet restore ./NopCommerce.sln
echo "-------------------------------------------------------"
echo ""

echo "-------------------------------------------------------"
echo "Building nopCommerce project..."
echo "-------------------------------------------------------"
cd $SOURCEROOT/Presentation/Nop.Web/
dotnet build -c Release
echo "-------------------------------------------------------"
echo ""

echo "-------------------------------------------------------"
echo "Building plugins..."
echo "-------------------------------------------------------"
cd $SOURCEROOT/Plugins/Nop.Plugin.Misc.SimpleCheckOut && dotnet build -c Release
cd $SOURCEROOT/Plugins/Nop.Plugin.Misc.Omnishop && dotnet build -c Release
cd $SOURCEROOT/Plugins/Nop.Plugin.Payments.CheckMoneyOrder && dotnet build -c Release
cd $SOURCEROOT/Plugins/Nop.Plugin.Tax.FixedOrByCountryStateZip && dotnet build -c Release
cd $SOURCEROOT/Plugins/Nop.Plugin.Shipping.FixedByWeightByTotal && dotnet build -c Release
cd $SOURCEROOT/Plugins/Nop.Plugin.Widgets.GoogleAnalytics && dotnet build -c Release

#Nop.Plugin.Pickup.PickupInStore
#Nop.Plugin.ExchangeRate.EcbExchange
#Nop.Plugin.DiscountRules.CustomerRoles
echo "-------------------------------------------------------"
echo ""

echo "-------------------------------------------------------"
echo "Publishing web project to folder: $PUBLISHROOT"
echo "-------------------------------------------------------"
cd $SOURCEROOT/Presentation/Nop.Web/ 
dotnet publish -c Release -o $PUBLISHROOT

