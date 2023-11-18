using DynamicPatcher;
using Extension.Ext;
using Extension.INI;
using Extension.Script;
using Microsoft.AspNetCore.SignalR.Client;
using PatcherYRpp;
using PatcherYRpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    [Serializable]
    [ScriptAlias(nameof(CricketControlScript))]
    public class CricketControlScript : TechnoScriptable
    {
        public CricketControlScript(TechnoExt owner) : base(owner)
        {
            if (connection == null)
            {
                connection = new HubConnectionBuilder()
                 .WithUrl("http://localhost:51732/GameHub?UserId=F5FCD02C-D49E-4D0E-9389-21452C4B5C7F")
                 .Build();

                connection.On<TechnoCreateInfo>("TechnoCreated", data =>
                {
                    lock (qLocker)
                    {
                        Cmds.Enqueue(new CricketCommand() { CricketCmdType = CricketCmdType.Create, Data = data });
                    }
                });

                connection.On("Hunted", () =>
                {
                    lock (qLocker)
                    {
                        Cmds.Enqueue(new CricketCommand() { CricketCmdType = CricketCmdType.Hunt });
                    }
                });

                connection.On("Destroyed", () =>
                {
                    lock (qLocker)
                    {
                        Cmds.Enqueue(new CricketCommand() { CricketCmdType = CricketCmdType.Destroy });
                    }
                });

                Task.Run(async () =>
                {
                    await connection.StartAsync();
                    await connection.InvokeAsync("GameStart");
                });
            }

        }

        INIComponentWith<CricketControlSetting> Ini;

        public static Queue<CricketCommand> Cmds = new Queue<CricketCommand>();

        public static HubConnection connection = null;


        public override void Awake()
        {
            Ini = this.CreateRulesIniComponentWith<CricketControlSetting>(Owner.OwnerObject.Ref.Type.Ref.Base.Base.ID);

        }

        public override void OnUpdate()
        {

            CricketCommand cmd = null;
            if (Cmds.Count > 0)
            {
                lock (qLocker)
                {
                    if (Cmds.Count > 0)
                    {
                        cmd = Cmds.Dequeue();
                    }
                }
            }

            if (cmd != null)
            {
                switch (cmd.CricketCmdType)
                {
                    case CricketCmdType.Create:
                        {
                            if(cmd.Data is TechnoCreateInfo info)
                            {
                                var pType = TechnoTypeClass.ABSTRACTTYPE_ARRAY.Find(info.TechnoId);
                                if (pType.IsNull)
                                {
                                    Logger.Log("technotype is null");
                                    break;
                                }

                                var hindex = info.House;


                                if (HouseClass.Observer.IsNotNull)
                                {
                                    if (HouseClass.Observer == HouseClass.Array[0])
                                        hindex = info.House + 1;
                                }

                                var house = HouseClass.Array[hindex];


                                if (house.IsNull)
                                {
                                    Logger.Log("house is null");
                                    break;
                                }

                                CoordStruct target = new CoordStruct(0, 0, 0);

                                if (info.House == 0)
                                {
                                    switch (info.TechnoPosition)
                                    {
                                        case TechnoPosition.Front:
                                            {
                                                target = Ini.Data.CoordStructFront1P;
                                                break;
                                            }
                                        case TechnoPosition.Middle:
                                            {
                                                target = Ini.Data.CoordStructMiddle1P;
                                                break;
                                            }
                                        case TechnoPosition.Back:
                                            {
                                                target = Ini.Data.CoordStructBack1P;
                                                break;
                                            }
                                        default:
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (info.TechnoPosition)
                                    {
                                        case TechnoPosition.Front:
                                            {
                                                target = Ini.Data.CoordStructFront2P;
                                                break;
                                            }
                                        case TechnoPosition.Middle:
                                            {
                                                target = Ini.Data.CoordStructMiddle2P;
                                                break;
                                            }
                                        case TechnoPosition.Back:
                                            {
                                                target = Ini.Data.CoordStructBack2P;
                                                break;
                                            }
                                        default:
                                            break;
                                    }
                                }


                                var targetCell = CellClass.Coord2Cell(target);

                                for (var i = 0; i < info.Count; i++)
                                {
                                    var techno = pType.Ref.Base.CreateObject(house).Convert<TechnoClass>();

                                    if (!TechnoPlacer.PlaceTechnoNear(techno, targetCell))
                                        return;

                                    if (info.Veterancy == 1)
                                    {
                                        techno.Ref.Veterancy.SetVeteran(true);
                                    }else if(info.Veterancy == 2)
                                    {
                                        techno.Ref.Veterancy.SetElite(true);
                                    }


                                    var mission = techno.Convert<MissionClass>();
                                    mission.Ref.ForceMission(Mission.Stop);
                                }
                            }
                            break;
                        }
                    case CricketCmdType.Hunt:
                        {
                            for (var i = 0; i < TechnoClass.Array.Count(); i++)
                            {
                                var item = TechnoClass.Array[i];
                                if (item.Ref.Owner.IsNull)
                                    continue;

                                var ownerName = item.Ref.Owner.Ref.Type.Ref.Base.ID.ToString();
                                if (ownerName == "Special" || ownerName == "Neutral")
                                    continue;

                                if (item.Ref.Base.Base.WhatAmI() == AbstractType.Building)
                                    continue;

                                var mission = item.Convert<MissionClass>();
                                mission.Ref.ForceMission(Mission.Hunt);
                            }
                            break;
                        }
                    case CricketCmdType.Destroy:
                        {
                            for (var i = TechnoClass.Array.Count() - 1; i > 0; i--)
                            {
                                var item = TechnoClass.Array[i];
                                if (item.Ref.Owner.IsNull)
                                    continue;

                                var ownerName = item.Ref.Owner.Ref.Type.Ref.Base.ID.ToString();
                                if (ownerName == "Special" || ownerName == "Neutral")
                                    continue;

                                var ext = TechnoExt.ExtMap.Find(item);
                                if (ext.GameObject.GetComponent<CricketMCVScript>() != null)
                                    continue;

                                if(item.Ref.Base.InLimbo)
                                {
                                    item.Ref.Base.RegisterKill(item.Ref.Owner);
                                    item.Ref.Base.UnInit();
                                }
                                else
                                {
                                    item.Ref.Base.TakeDamage(10000, WarheadTypeClass.ABSTRACTTYPE_ARRAY.Find("Super"), false);
                                }
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        private static readonly object qLocker = new object();



    }

    [Serializable]
    [ScriptAlias(nameof(CricketMCVScript))]
    public class CricketMCVScript : TechnoScriptable
    {
        public CricketMCVScript(TechnoExt owner) : base(owner)
        {
        }
    }


    [Serializable]
    public class CricketCommand
    {
        public CricketCmdType CricketCmdType { get; set; }

        public object Data { get; set; }
    }

    public enum CricketCmdType
    {
        Create,
        Hunt,
        Destroy
    }

    [Serializable]
    public class TechnoCreateInfo
    {
        public string TechnoId { get; set; }
        //玩家
        public int House { get; set; } = 0;

        public int Count { get; set; } = 1;

        public int Veterancy { get; set; } = 0;

        public TechnoPosition TechnoPosition { get; set; }
    }

    public enum TechnoPosition
    {
        Front,
        Middle,
        Back
    }

    public class CricketControlSetting : INIAutoConfig
    {
        [INIField(Key = "CricketControl.CoordStructFront1P")]
        public CoordStruct CoordStructFront1P;
        [INIField(Key = "CricketControl.CoordStructMiddle1P")]
        public CoordStruct CoordStructMiddle1P;
        [INIField(Key = "CricketControl.CoordStructBack1P")]
        public CoordStruct CoordStructBack1P;

        [INIField(Key = "CricketControl.CoordStructFront2P")]
        public CoordStruct CoordStructFront2P;
        [INIField(Key = "CricketControl.CoordStructMiddle2P")]
        public CoordStruct CoordStructMiddle2P;
        [INIField(Key = "CricketControl.CoordStructBack2P")]
        public CoordStruct CoordStructBack2P;

    }

}
