#!/bin/bash
BUILDROOT=/home/pdadmin/nopPlantedamp-build
SOURCEROOT=$BUILDROOT/repo/src
PUBLISHDIR=/var/www/nopPlantedamp-release-$(date +%Y%m%d-%H%M%S)

cd $BUILDROOT
echo "-------------------------------------------------------"
echo "Pulling updated source from Git remote..."
echo "-------------------------------------------------------"
#TODO: If not exists: git clone?
cd ./repo/
git pull
echo "-------------------------------------------------------"
echo ""

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
echo "Publishing web project to folder: $PUBLISHDIR"
echo "-------------------------------------------------------"
cd $SOURCEROOT/Presentation/Nop.Web/
dotnet publish -c Release -o $PUBLISHDIR

echo "Copying config files to $PUBLISHDIR/App_Data"
\cp -v -r $BUILDROOT/config/App_Data/. $PUBLISHDIR/App_Data/.

echo "Setting owner and permissions for $PUBLISHDIR"
chown -R nopPlantedamp-app-user:nogroup $PUBLISHDIR/
chown -R nopPlantedamp-app-user:nogroup $PUBLISHDIR/.
chmod -R u=rxw,og=rx $PUBLISHDIR/
