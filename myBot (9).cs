using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pirates;

namespace MyBot
{
    enum Task { DefenceEnemyIsland, DefenceHomeIsland, Attack, Runner };
    public class MyBot : IPirateBot
    {
        public static PirateGame game; // good.
        private static List<Pirates> ourPirates = new List<Pirates>();

        public void DoTurn(PirateGame game)
        {
            MyBot.game = game;
            List<Task> tasks = Predict.GetTasks();
            if (game.GetTurn() == 1)
            {
                int mone = 0;
                foreach (Pirate pirate in game.GetAllMyPirates())
                {
                    ourPirates.Add(new Pirates(pirate, tasks[mone % tasks.Count]));
                    mone++;
                }
            }
            for (int i = 0; i < game.GetAllMyPirates().Count; i++)
            {
                ourPirates[i].Pirate = game.GetAllMyPirates()[i];
                ourPirates[i].Act();
            }
            List<Drones>[] organizedDrones = new List<Drones>[game.GetAllIslands().Count];
            for (int i = 0; i < organizedDrones.Length; i++)
            {
                organizedDrones[i] = new List<Drones>();
            }
            foreach (Drone drone in game.GetMyLivingDrones())
            {
                Add(drone, organizedDrones[Id(drone.InitialLocation)]);
            }
        }

        private int Id(Location initialLocation)
        {
            foreach (Island island in game.GetAllIslands())
                if (island.Location.Equals(initialLocation))
                    return island.Id;
            return 0;
        }

        private void Add(Drone drone, List<Drones> list)
        {
            if (list.Count == 0)
                list.Add(new Drones(drone, drone.InitialLocation));
            else
            {
                if (list[list.Count - 1].Act(drone) == 0)
                    list.Add(new Drones(drone, drone.InitialLocation));
            }
        }
    }

    class Predict
    {
        public static List<Pirate> enemies = MyBot.game.GetAllEnemyPirates();

        public static List<Task> GetTasks()
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Runner);
            tasks.Add(Task.DefenceEnemyIsland);
            tasks.Add(Task.DefenceHomeIsland);
            tasks.Add(Task.Attack);
            return tasks;
        }

        /// <summary>
        /// Find if there is an enemy on our home.
        /// </summary>
        /// <returns>true if there is, flase if there isn't</returns>
        public static bool EnemyHome()
        {
            List<Pirate> enemies = MyBot.game.GetEnemyLivingPirates();
            City Home = MyBot.game.GetMyCities()[0];
            foreach (Pirate enemy in enemies)
                if (Home.InRange(enemy.Location, Home.UnloadRange))
                    return true;
            return false;
        }

        /// <summary>
        /// For the pirate who goes to the enemy city to kill drones.
        /// </summary>
        /// <returns>The location he needs to go to</returns>
        public static Location DefenceEnemy()
        {
            City enemy = MyBot.game.GetEnemyCities()[0];
            foreach (Drone drone in MyBot.game.GetEnemyLivingDrones())
                if (enemy.InRange(drone, enemy.UnloadRange * 4))
                    return drone.Location;
            return enemy.Location;
        }

        /// <summary>
        /// Find out which target is the closest one to the location.
        /// </summary>
        /// <returns>the target.</returns>
        public static GameObject GetClosestToLocation(List<GameObject> targets, Location location)
        {
            GameObject closest = targets[0];

            foreach (GameObject target in targets)
                if (location.Distance(target) < location.Distance(closest))
                    closest = target;

            return closest;
        }

        /// <summary>
        /// Find which island is the recommendable island.
        /// </summary>
        /// <param name="howManyShip">how meny runners we got avaible</param>
        /// <param name="location">the pirate location</param>
        /// <returns></returns>
        public static GameObject WhichIsland(int howManyShip, Location location)
        {
            List<GameObject> targets = ConvertIslandsToGameObjectList(MyBot.game.GetNeutralIslands());

            if (targets.Count > 0)
                return GetClosestToLocation(targets, location);

            GameObject target = GetEnemyIslandMinPirates();

            if (target == null)
                return null;

            if (GetEnemyPiratesNum(target.Location) <= howManyShip)
                return target;

            return null;
        }

        /// <summary>
        /// find out which enemy island has the smallest number of pirates.
        /// </summary>
        /// <returns>the island</returns>
        public static Island GetEnemyIslandMinPirates()
        {
            List<Island> islands = MyBot.game.GetEnemyIslands();
            if (islands.Count == 0)
                return null;
            Island min = islands[0];
            int minNum = GetEnemyPiratesNum(min.Location);

            foreach (Island island in islands)
            {
                int num = GetEnemyPiratesNum(island.Location);
                if (num < minNum)
                {
                    min = island;
                    minNum = num;
                }
            }

            return min;
        }

        /// <summary>
        /// Find who many enemy pirates are on that location.
        /// </summary>
        /// <param name="location">the location that is being checked.</param>
        /// <returns>the number</returns>
        public static int GetEnemyPiratesNum(Location location)
        {
            int mone = 0;

            foreach (Pirate enemy in enemies)
                if (enemy.IsAlive() && enemy.Distance(location) < 2)
                    mone++;

            return mone;
        }

        /// <summary>
        /// converts an islands list to a GameObject list.
        /// </summary>
        /// <param name="islands">the islands list to be converted.</param>
        /// <returns>the GameObject list</returns>
        public static List<GameObject> ConvertIslandsToGameObjectList(List<Island> islands)
        {
            List<GameObject> list = new List<GameObject>();

            foreach (Island island in islands)
                list.Add(island);

            return list;
        }

        /// <summary>
        /// converts an aircraft list to a GameObject list.
        /// </summary>
        /// <param name="aircrafts">the aircraft list to be converted.</param>
        /// <returns>the GameObject list</returns>
        public static List<GameObject> ConvertAircraftsToGameObjectList(List<Aircraft> aircrafts)
        {
            List<GameObject> list = new List<GameObject>();

            foreach (Aircraft aircraft in aircrafts)
                list.Add(aircraft);

            return list;
        }

        /// <summary>
        /// converts a drones list to a GameObject list.
        /// </summary>
        /// <param name="drones">the drone list to be converted.</param>
        /// <returns>the GameObject list</returns>
        public static List<GameObject> ConvertDronesToGameObjectList(List<Drone> drones)
        {
            List<GameObject> list = new List<GameObject>();

            foreach (Drone drone in drones)
                list.Add(drone);

            return list;
        }

        /// <summary>
        /// Need to be smart.
        /// </summary>
        /// <returns>the number of drones in a group</returns>
        public static int GetDronesGroupedNum(Location initialLocation, Location currentLocation)
        {
            if (!currentLocation.Equals(initialLocation))
                return 1;
            foreach (Pirate enemy in MyBot.game.GetEnemyLivingPirates())
                if (initialLocation.InRange(enemy, enemy.MaxSpeed * 3))
                    return 1;
            int turns = MyBot.game.GetAllIslands()[Id(initialLocation)].TurnsToDroneCreation;
            return 5 / turns + 2; // 5 devided by the number of turns to drone creation plus one.
        }

        private static int Id(Location initialLocation)
        {
            foreach (Island island in MyBot.game.GetAllIslands())
                if (island.Location.Equals(initialLocation))
                    return island.Id;
            return 0;
        }
    }

    class Pirates
    {
        public delegate void Action();
        private Pirate pirate;
        private Action action;
        public static int runners = 0;
        public static int defenceEnemy = 0;
        public static int defenceHome = 0;
        public static int attack = 0;

        public Pirate Pirate { get { return pirate; } set { pirate = value; } }

        /// <summary>
        /// constructor, getting the pirate and his task.
        /// </summary>
        /// <param name="pirate">the pirate.</param>
        /// <param name="task">his task</param>
        public Pirates(Pirate pirate, Task task)
        {
            this.Pirate = pirate;
            switch (task)
            {
                case Task.DefenceEnemyIsland:
                    action = DefenceEnemy;
                    defenceEnemy++;
                    break;
                case Task.Attack:
                    action = Attack;
                    attack++;
                    break;
                case Task.DefenceHomeIsland:
                    action = DefenceHome;
                    defenceHome++;
                    break;
                case Task.Runner:
                    action = Runner;
                    runners++;
                    break;
            }
        }

        /// <summary>
        /// Invoking the pirate action, if he is alive and didn't attack anyone.
        /// </summary>
        public void Act()
        {
            if (Pirate.IsAlive())
                if (!TryAttack())
                    action.Invoke();
        }

        /// <summary>
        /// Moves the pirate to our home if the opponent is there, otherwise calls Attack.
        /// </summary>
        private void DefenceHome()
        {
            if (Predict.EnemyHome())
                MovePirate(MyBot.game.GetMyCities()[0].Location);
            else
            {
                if (MyBot.game.GetMyLivingDrones().Count != 0)
                    ProtectDrones();  // may be changed.
                else
                    Runner();
            }
        }

        private void ProtectDrones()
        {
            List<GameObject> targets = Predict.ConvertDronesToGameObjectList(MyBot.game.GetMyLivingDrones());
            GameObject target = Predict.GetClosestToLocation(targets, Pirate.Location);
            MovePirate(target.Location);
        }

        /// <summary>
        /// Move the pirate that goes to the enemy cities and destroies drones.
        /// </summary>
        private void DefenceEnemy()
        {
            if (MyBot.game.GetEnemyIslands().Count == 0)
            {
                Attack();
                return;
            }
            MovePirate(Predict.DefenceEnemy());
        }

        /// <summary>
        /// Move the runners to the closest island to home, if the closest island isn't ours or he doesn't 
        /// have another good option.
        /// </summary>
        private void Runner()
        {
            List<GameObject> islands = Predict.ConvertIslandsToGameObjectList(MyBot.game.GetAllIslands());
            GameObject closest = Predict.GetClosestToLocation(islands, Pirate.Location);
            int numClosest = Predict.GetEnemyPiratesNum(closest.Location);

            if (!closest.Owner.Equals(MyBot.game.GetMyself()) && numClosest <= runners)
            {
                MovePirate(closest.GetLocation());
                return;
            }

            GameObject island = Predict.WhichIsland(runners, Pirate.Location);
            if (island == null)
            {
                if (numClosest > runners)
                {
                    Attack(); // May be changed.
                    return;
                }
                MovePirate(closest.GetLocation());
                return;
            }
            MovePirate(island.Location);
        }

        private void Attack()
        {
            if (attack > 1)
            {
                // DoubleAttack();
                // return;
            }
            List<GameObject> targets = Predict.ConvertAircraftsToGameObjectList(MyBot.game.GetEnemyLivingAircrafts());
            GameObject target = Predict.GetClosestToLocation(targets, Pirate.Location);
            MovePirate(target.Location);
        }

        private void DoubleAttack()
        {

        }

        /// <summary>
        /// moving the pirate to the destination if he isn't there.
        /// </summary>
        /// <param name="destination">the location we want to send the pirate to</param>
        private void MovePirate(Location destination)
        {
            if (!Pirate.GetLocation().Equals(destination))
            {
                List<Location> sailOptions = MyBot.game.GetSailOptions(Pirate, destination);
                MyBot.game.SetSail(Pirate, sailOptions[0]);
                MyBot.game.Debug("pirate " + Pirate + " sails to " + sailOptions[0]);
            }
        }

        /// <summary>
        /// tries to attack enemy aircraft.
        /// </summary>
        /// <returns>true if he attacked, false otherwise</returns>
        public bool TryAttack()
        {
            foreach (Aircraft enemy in MyBot.game.GetEnemyLivingAircrafts())
            {
                if (Pirate.InAttackRange(enemy))
                {
                    MyBot.game.Attack(Pirate, enemy);
                    MyBot.game.Debug("pirate " + Pirate + " attacks " + enemy);
                    return true;
                }
            }
            return false;
        }

    }

    class Drones
    {
        private List<Drone> drones;
        private Location creationIsland;
        private int dronesNum;

        public Drones(Drone drone, Location creationIsland)
        {
            this.drones = new List<Drone>();
            drones.Add(drone);
            this.creationIsland = creationIsland;
            dronesNum = Predict.GetDronesGroupedNum(creationIsland, drone.Location);
        }

        public int Act(Drone drone)
        {
            if (IsIn(drone))
                return -1;
            if (drones.Count == dronesNum)
            {
                Move();
                return 0;
            }
            if (Add(drone))
                return 1;
            return 0;
        }

        public bool IsIn(Drone drone)
        {
            foreach (Drone d in drones)
                if (d == drone)
                    return true;
            return false;
        }

        public bool Add(Drone drone)
        {
            if (!drone.InitialLocation.Equals(creationIsland.GetLocation()))
                return false;
            drones.Add(drone);
            return true;
        }

        public void Move()
        {
            foreach (Drone drone in drones)
                MoveDrone(drone, MyBot.game.GetMyCities()[0].Location);
        }

        /// <summary>
        /// moving the drone to the destination if he isn't there.
        /// </summary>
        /// <param name="destination">the location we want to send the drone to</param>
        private void MoveDrone(Drone drone, Location destination)
        {
            if (!drone.GetLocation().Equals(destination))
            {
                List<Location> sailOptions = MyBot.game.GetSailOptions(drone, destination);
                MyBot.game.SetSail(drone, sailOptions[0]);
                MyBot.game.Debug("pirate " + drone + " sails to " + sailOptions[0]);
            }
        }
    }
}
