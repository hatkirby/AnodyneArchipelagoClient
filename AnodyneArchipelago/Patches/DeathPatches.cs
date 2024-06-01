using AnodyneSharp;
using AnodyneSharp.Entities;
using AnodyneSharp.Entities.Enemy;
using AnodyneSharp.Entities.Enemy.Apartment;
using AnodyneSharp.Entities.Enemy.Bedroom;
using AnodyneSharp.Entities.Enemy.Circus;
using AnodyneSharp.Entities.Enemy.Crowd;
using AnodyneSharp.Entities.Enemy.Go;
using AnodyneSharp.Entities.Enemy.Hotel;
using AnodyneSharp.Entities.Enemy.Hotel.Boss;
using AnodyneSharp.Entities.Enemy.Redcave;
using AnodyneSharp.MapData;
using AnodyneSharp.MapData.Tiles;
using HarmonyLib;
using System.Reflection;

namespace AnodyneArchipelago.Patches
{
    [HarmonyPatch]
    static class ArthurDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Circus.CircusFolks").GetNestedType("Arthur", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was suplexed by Arthur.";
            }
        }
    }

    [HarmonyPatch(typeof(BaseSpikeRoller), nameof(BaseSpikeRoller.Collided))]
    static class SpikeRollerDamagePatch
    {
        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was crushed by a spike roller.";
            }
        }
    }

    [HarmonyPatch]
    static class WatcherDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Apartment.BaseSplitBoss").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was immolated by the Watcher.";
            }
        }
    }

    [HarmonyPatch]
    static class RockCreatureDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Interactive.Npc.RedSea.BombDude").GetMethod("PlayerInteraction");
        }

        static void Postfix()
        {
            Plugin.ArchipelagoManager.DeathLinkReason = "was blown up by a rock creature and it's their own fault (sorry lmao).";
        }
    }

    [HarmonyPatch(typeof(BriarBossBody), nameof(BriarBossBody.Collided))]
    static class BriarBodyDamagePatch
    {
        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was repelled by the Briar.";
            }
        }
    }

    [HarmonyPatch]
    static class WatcherBulletDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Apartment.SplitBoss").GetNestedType("Bullet", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was shot by the Watcher.";
            }
        }
    }

    [HarmonyPatch]
    static class WallBulletDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Crowd.Face").GetNestedType("Bullet", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was shot by the Wall.";
            }
        }
    }

    [HarmonyPatch]
    static class BriarBulletDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(BriarBossBody).GetNestedType("Bullet", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was shot by the Briar.";
            }
        }
    }

    [HarmonyPatch]
    static class ManagerLandPhaseBulletDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(LandPhase).GetNestedType("Bullet", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was shot by the Manager on land.";
            }
        }
    }

    [HarmonyPatch]
    static class ManagerWaterPhaseBulletDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(WaterPhase).GetNestedType("Bullet", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was shot by the Manager in the pool.";
            }
        }
    }

    [HarmonyPatch]
    static class FourShooterBulletDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Redcave.Four_Shooter").GetNestedType("Bullet", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was shot by a four-shooter.";
            }
        }
    }

    [HarmonyPatch]
    static class SlimeBulletDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(Slime).GetNestedType("Bullet", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was hit by a glob of red slime.";
            }
        }
    }

    [HarmonyPatch(typeof(Slime), nameof(Slime.Collided))]
    static class SlimeDamagePatch
    {
        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was absorbed by slime.";
            }
        }
    }

    [HarmonyPatch]
    static class FrogBulletDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Crowd.Frog").GetNestedType("BurstBullet", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was burped on by a frog.";
            }
        }
    }

    [HarmonyPatch(typeof(Burst_Plant), nameof(Burst_Plant.Collided))]
    static class FlowerDamagePatch
    {
        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "tripped on a flower.";
            }
        }
    }

    [HarmonyPatch]
    static class FlowerBulletDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(Burst_Plant).GetNestedType("BurstBullet", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "had an allergic reaction to pollen.";
            }
        }
    }

    [HarmonyPatch]
    static class ChaserDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Cell.Chaser").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was hugged to death by a Chaser.";
            }
        }
    }

    [HarmonyPatch]
    static class ServantsDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Circus.CircusFolks").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was tossed into a pit by Arthur and Javiera.";
            }
        }
    }

    [HarmonyPatch]
    static class ContortDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Circus.Contort").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "got majorly clowned on.";
            }
        }
    }

    [HarmonyPatch]
    static class ContortSmallDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Circus.Contort").GetNestedType("ContortSmall", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was juggled like the football.";  // You kick Miette??
            }
        }
    }

    [HarmonyPatch]
    static class DashTrapDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Apartment.DashTrap").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was impaled by a dash trap.";
            }
        }
    }

    [HarmonyPatch(typeof(Dog), nameof(Dog.Collided))]
    static class DogDamagePatch
    {
        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was bitten by a dog.";
            }
        }
    }

    [HarmonyPatch]
    static class DustMaidDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Hotel.Dustmaid").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was sanitized by a dust maid.";
            }
        }
    }

    [HarmonyPatch]
    static class AnnoyerFireballDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(Annoyer).GetNestedType("Fireball", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was scorched by an annoyer.";
            }
        }
    }

    [HarmonyPatch(typeof(Annoyer), nameof(Annoyer.Collided))]
    static class AnnoyerDamagePatch
    {
        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was swooped by an annoyer.";
            }
        }
    }

    [HarmonyPatch(typeof(Lion), nameof(Lion.Collided))]
    static class LionDamagePatch
    {
        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was eaten by a lion.";
            }
        }
    }

    [HarmonyPatch(typeof(Lion.Fireball), nameof(Lion.Fireball.Collided))]
    static class LionFireballDamagePatch
    {
        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was toasted by a lion.";
            }
        }
    }

    [HarmonyPatch]
    static class BriarFireballDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Go.BlueThorn").GetNestedType("Fireball").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was torched by the Briar.";
            }
        }
    }

    [HarmonyPatch]
    static class FrogDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Crowd.Frog").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was sat on by a frog.";
            }
        }
    }

    [HarmonyPatch(typeof(GasGuy), nameof(GasGuy.Collided))]
    static class GasGuyDamagePatch
    {
        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "suffocated in the fumes.";
            }
        }
    }

    [HarmonyPatch]
    static class WallHandDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Crowd.Hand").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was punched by the Wall.";
            }
        }
    }

    [HarmonyPatch]
    static class BriarIceCrystalDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Go.HappyThorn").GetNestedType("IceCrystal").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was bowled over by the Briar.";
            }
        }
    }

    [HarmonyPatch]
    static class SageBulletDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Etc.SageBoss").GetNestedType("HurtingEntity", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was shot by the Sage.";
            }
        }
    }

    [HarmonyPatch]
    static class JavieraDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Circus.CircusFolks").GetNestedType("Javiera", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was vaulted by Javiera.";
            }
        }
    }

    [HarmonyPatch(typeof(LandPhase), nameof(LandPhase.Collided))]
    static class ManagerLandPhaseDamagePatch
    {
        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was trampled by the Manager.";
            }
        }
    }

    [HarmonyPatch]
    static class PewPewDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(PewLaser).GetNestedType("Laser", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "got pew pew'd.";
            }
        }
    }

    [HarmonyPatch]
    static class WallLaserDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Crowd.Laser").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was flattened by the Wall.";
            }
        }
    }

    [HarmonyPatch]
    static class OnOffLaserDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(OnOffLaser).GetNestedType("Laser", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was evaporated by a laser.";
            }
        }
    }

    [HarmonyPatch(typeof(Map), "CollideTile")]
    static class MapCollideTileDamagePatch
    {
        static void Postfix(Tile t, Entity ent)
        {
            if (ent is Player && t.collisionEventType == CollisionEventType.SPIKE && (ent as Player).state != PlayerState.AIR)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "fell into some spikes.";
            }
        }
    }

    [HarmonyPatch(typeof(Player), "SinkingLogic")]
    static class PlayerSinkingDamagePatch
    {
        static void Postfix(Player __instance)
        {
            if (__instance.y_push == 0.0f)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "drowned.";
            }
        }
    }

    [HarmonyPatch]
    static class RatDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Apartment.Rat").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "got spooked by a cute little rat squeaking too loudly.";
            }
        }
    }

    [HarmonyPatch(typeof(Red_Boss), nameof(Red_Boss.Collided))]
    static class RogueDamagePatch
    {
        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was traumatized by the Rogue.";
            }
        }
    }

    [HarmonyPatch]
    static class RotatorDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Crowd.RotatorBullet").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was pierced by a rotator bullet.";
            }
        }
    }

    [HarmonyPatch]
    static class SageDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Etc.SageBoss").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was judged by the Sage.";
            }
        }
    }

    [HarmonyPatch(typeof(Seer), nameof(Seer.Collided))]
    static class SeerDamagePatch
    {
        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was menaced by the Seeing One.";
            }
        }
    }

    [HarmonyPatch]
    static class SeerOrbDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Bedroom.SeerOrb").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was pummelled by the Seeing One.";
            }
        }
    }

    [HarmonyPatch]
    static class SeerWaveDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Bedroom.SeerWave").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was flattened by the Seeing One.";
            }
        }
    }

    [HarmonyPatch(typeof(Shieldy), nameof(Shieldy.Collided))]
    static class ShieldyDamagePatch
    {
        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was pushed away by a Shieldy.";
            }
        }
    }

    [HarmonyPatch]
    static class ServantsShockWaveDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Circus.CircusFolks").GetNestedType("ShockWave", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was blasted by Arthur and Javiera.";
            }
        }
    }

    [HarmonyPatch(typeof(Silverfish), nameof(Silverfish.Collided))]
    static class SilverfishDamagePatch
    {
        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was swarmed by silverfish.";
            }
        }
    }

    [HarmonyPatch]
    static class SlasherDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(Slasher).GetNestedType("Slash", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was cleaved in twain by a Slasher.";
            }
        }
    }

    [HarmonyPatch]
    static class RogueSplashDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Redcave.SplashBullet").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "got splashed by the Rogue.";
            }
        }
    }

    [HarmonyPatch]
    static class KillerDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Suburb.SuburbKiller").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "got killed. Yeah.";
            }
        }
    }

    [HarmonyPatch]
    static class TeleGuyDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Apartment.TeleGuy").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "got sneaked up on by a Tele Guy.";
            }
        }
    }

    [HarmonyPatch]
    static class RogueTentacleDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Redcave.Tentacle").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was squeezed to death by the Rogue.";
            }
        }
    }

    [HarmonyPatch]
    static class BriarThornDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(BriarBossBody).GetNestedType("Thorn", BindingFlags.NonPublic).GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "was pierced by the Briar.";
            }
        }
    }

    [HarmonyPatch]
    static class ThornGateDamagePatch
    {
        static MethodInfo TargetMethod()
        {
            return typeof(AnodyneGame).Assembly.GetType("AnodyneSharp.Entities.Enemy.Go.ThornGate").GetMethod("Collided");
        }

        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "stumbled into the Briar's thorns.";
            }
        }
    }

    [HarmonyPatch(typeof(WaterPhase), nameof(WaterPhase.Collided))]
    static class ManagerWaterPhaseDamagePatch
    {
        static void Postfix(Entity other)
        {
            if (other is Player)
            {
                Plugin.ArchipelagoManager.DeathLinkReason = "drowned in the hotel pool.";
            }
        }
    }
}
