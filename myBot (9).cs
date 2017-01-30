using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pirates;

namespace MyBot
{
    public class MyBot : Pirates.IPirateBot
    {
        public void DoTurn(PirateGame game)
        {
            HandlePirates0_1(game);
            HandlePirates2_3(game);
            HandlePirates4(game);
            // Give orders to my pirates
            // Give orders to my drones
            HandleDrones(game);
            //
        }

        private void HandlePirates0_1(PirateGame game)
        {
            // Go over all of my pirates

            Pirate ListPirate = game.GetAllMyPirates();
            int HowManyPirates = ListPirate.Count - 1;
            int piratesAlreadeSend = 0;// 0
            const int numStrategy = 3;//number of strategy consot
            int piratestosend = 0;//it will consist- untill which part of the list of pirate to sent  
            //we will be send from piratespiratesAlreadeSend to piratestosend for each strategy
            for (int i = 1; i <= numStrategy; i++)
            {
                piratesAlreadeSend = piratestosend ;
                piratestosend = piratestosend + UpdateNum(i, HowManyPirates, piratesAlreadeSend);
                SendToStrategy(i, ListPirate, piratesAlreadeSend, piratestosend);
            }
            CheckLeftAndSend(piratestosend, HowManyPirates, ListPirate);
        }
        private int UpdateNum(int numStrategy, int HowManyPirates, int sumpiratesAlreadeSend)
        {
            int precent = WhatPrecent(numStrategy);
            return Math.Round(HowManyPirates * precent);
        }

        private void CheckLeftAndSend(int numSent, int HowManyPirates, List<Pirate> ListPirate)
        {
            numStrategyToSend = 3;//example
            if (numSent < HowManyPirates)
                SendToStrategy(numStrategyToSend, ListPirate, numSent, HowManyPirates);
        }

        private double WhatPrecent(int numStrategy)
        {

            if (numStrategy == 1)
                return 0.4;
            else if (numStrategy == 2)
                return 0.2;
            else
                return 0.4;
        }
        private void SendToStrategy(int numStrategyToSend, List<Pirate> ListPirate,int from,int untill)
            {

            if (numStrategyToSend == 1)
                Strategy1(ListPirate,  from,  untill);//first strategy
            else if (numStrategyToSend == 2)
                Strategy2(ListPirate,  from,  untill);//secend strategy
            else
                Strategy3(ListPirate,  from, untill);//third strategy
        }
//____________________________________________________________________________________________________________
//_________________________________________________________________________________________________________________
        }
        private void SailToDestination(Location destination, Pirate pirate, PirateGame game) // Maybe Later We Will Change Island To MapObject
        {
            List<Location> sailOptions = game.GetSailOptions(pirate, destination);
            // Set sail!
            game.SetSail(pirate, sailOptions[0]);
            // Print a message
            game.Debug("pirate " + pirate + " sails to " + sailOptions[0]);
        }

    private void Strategy1(List<Pirate> ListPirate, int from, int untill)
    {//this fanc need to decide which destination
        Island destination = game.GetAllIslands()[3];

        for (int i = from; i < untill; i++)
        {
            Pirate pirate = ListPirate[i];
            if (!IsMyIsland(destination, game)) // Check If The Island That We Want To Go Is Not Ours
            {
                SailToDestination(destination.Location, pirate);
            }
            else
            {
                if (pirate.GetLocation().Distance(new Location(24, 19)) != 0)
                {
                    Location destination1 = new Location(24, 19);
                    SailToDestination(destination1, pirate, game);
                }

                else
                {
                    if (pirate.GetLocation().Distance(new Location(24, 26)) != 0)
                    {
                        Location destination2 = new Location(24, 26);
                        SailToDestination(destination2, pirate, game);
                    }
                }
            }
        }
    }
        private void HandlePirates2_3(PirateGame game)
        {
            // Go over all of my pirates
            for (int i = 2; i < 4; i++)
            {
                Pirate pirate = game.GetAllMyPirates()[i];
                if (pirate.IsAlive())
                {
                    if (!TryAttack(pirate, game))
                    {

                        // Get the first island
                        Island destination = WhichIsland(1, game);
                        if (destination == null)
                        {
                            Location destination1 = new Location(14, 23);
                            List<Location> sailOptions1 = game.GetSailOptions(pirate, destination1);
                            game.SetSail(pirate, sailOptions1[0]);
                        }
                        else
                        {
                            // Get sail options
                            List<Location> sailOptions = game.GetSailOptions(pirate, destination);
                            // Set sail!
                            game.SetSail(pirate, sailOptions[0]);
                            // Print a message
                        }
                    }
                }
            }
        }

        //private void HandlePirates3(PirateGame game)
        //{
        //    Pirate pirate = game.GetAllMyPirates()[3];

        //    if (pirate.IsAlive())
        //    {
        //        if (!TryAttack(pirate, game))
        //        {
        //            Location destination = new Location(24, 11);
        //            List<Location> sailOptions = game.GetSailOptions(pirate, destination);
        //            game.SetSail(pirate, sailOptions[0]);
        //        }
        //    }
        //}

        private void HandlePirates4(PirateGame game)
        {
            Pirate pirate = game.GetAllMyPirates()[4];

            if (pirate.IsAlive())
            {
                if (!TryAttack(pirate, game))
                {
                    List<Drone> alled = game.GetEnemyLivingDrones();
                    if (alled.Count > 0)
                    {
                        if (alled[0].GetLocation().Col == game.GetEnemyCities()[0].GetLocation().Col)
                        {
                            Location destination1 = new Location(22, 38);
                            List<Location> sailOptions = game.GetSailOptions(pirate, destination1);
                            game.SetSail(pirate, sailOptions[0]);
                        }
                    }
                    else
                    {
                        Location destination = new Location(24, 34);
                        List<Location> sailOptions = game.GetSailOptions(pirate, destination);
                        game.SetSail(pirate, sailOptions[0]);
                    }
                }
            }
        }

        private bool IsMyIsland(Island i1, PirateGame game)
        {
            List<Island> listtest = game.GetMyIslands();
            for (int i = 0; i < listtest.Count; i++)
            {
                if (i1 == listtest[i])
                {
                    return true;
                }
            }

            return false;
        }

        private Island WhichIsland(int howManyShip, PirateGame game)
        {
            List<Island> isla = game.GetNeutralIslands();
            if (isla.Count > 0)
                return isla[0];
            List<Island> isla1 = game.GetEnemyIslands();
            for (int i = 0; i < isla1.Count; i++)
            {
                Island isa = isla1[i];
                List<Pirate> pi = game.GetEnemyLivingPirates();
                int mone = 0;
                for (int k = 0; k < pi.Count; k++)
                {
                    if (pi[k].Distance(isa) < 2)
                        mone++;

                }
                if (mone <= howManyShip)
                    return isa;
            }

            return null;
        }

        private void HandleDrones(PirateGame game)
        {
            // Go over all of my drones
            foreach (Drone drone in game.GetMyLivingDrones())
            {

                // Get my first city
                City destination = game.GetMyCities()[0];
                // Get sail options
                if (drone.GetLocation().Col == destination.GetLocation().Col)
                {
                    List<Location> sailOptions = game.GetSailOptions(drone, destination);
                    // Set sail!
                    game.SetSail(drone, sailOptions[0]);
                }
                else
                {
                    Location destination1 = new Location(drone.GetLocation().Row, destination.GetLocation().Col);
                    List<Location> sailOptions = game.GetSailOptions(drone, destination1);
                    // Set sail!
                    game.SetSail(drone, sailOptions[0]);
                }
            }
        }

        public bool TryAttack(Pirate pirate, PirateGame game)
        {
            // Go over all enemies
            foreach (Aircraft enemy in game.GetEnemyLivingAircrafts())
            {
                // Check if the enemy is in attack range
                if (pirate.InAttackRange(enemy))
                {
                    // Fire!
                    game.Attack(pirate, enemy);
                    // Print a message
                    game.Debug("pirate " + pirate + " attacks " + enemy);
                    // Did attack
                    return true;
                }
            }

            // Didnt attack
            return false;
        }
    }
}