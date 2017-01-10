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
            for (int i = 0; i < 2; i++)
            {
                Pirate pirate = game.GetAllMyPirates()[i];
                if (pirate.IsAlive())
                {
                    if (!TryAttack(pirate, game))
                    {
                        // Get the first island
                        Island destination = game.GetAllIslands()[3];
                        // Get sail options
                        if (!IsMyIsland(destination, game))
                        {
                            List<Location> sailOptions = game.GetSailOptions(pirate, destination);
                            // Set sail!
                            game.SetSail(pirate, sailOptions[0]);
                            // Print a message
                            game.Debug("pirate " + pirate + " sails to " + sailOptions[0]);
                        }
                        else
                        {
                            if (i == 0)
                            {
                                if (pirate.GetLocation().Distance(new Location(24, 19)) != 0)
                                {
                                    Location destination1 = new Location(24, 19);
                                    List<Location> sailOptions1 = game.GetSailOptions(pirate, destination1);
                                    game.SetSail(pirate, sailOptions1[0]);
                                }
                            }
                            else
                            {
                                if (pirate.GetLocation().Distance(new Location(24, 26)) != 0)
                                {
                                    Location destination1 = new Location(24, 26);
                                    List<Location> sailOptions1 = game.GetSailOptions(pirate, destination1);
                                    game.SetSail(pirate, sailOptions1[0]);
                                }
                            }
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