using Extension.CY;
using Extension.Ext;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace Extension.CWUtilities
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed class INILoadActionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed class SaveActionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed class LoadActionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed class AwakeActionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed class UpdateActionAttribute:Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed class PutActionAttribute : Attribute
    {
    }


    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed class RemoveActionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed class FireActionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed class ReceiveDamageActionAttribute : Attribute
    {
    }



    public static class PartialHelper
    {

        public static Action<TechnoGlobalExtension, CoordStruct, Direction> TechnoPutAction = (owner,coord,faceDir) => { };

        public static Action<TechnoGlobalExtension> TechnoAwakeAction = (owner) => { };


        public static Action<TechnoGlobalExtension> TechnoUpdateAction = (owner) => { };

        public static Action<TechnoGlobalExtension> TechnoRemoveAction = (owner) => { };

        public static Action<TechnoGlobalExtension, Pointer<AbstractClass>, int> TechnoFireAction = (owner,pTarget, weaponIndex) => { };

        public static Action<TechnoGlobalExtension, Pointer<int>,int ,Pointer<WarheadTypeClass>,Pointer<ObjectClass>,bool,bool,Pointer<HouseClass>> TechnoReceiveDamageAction = (owner,pDamage,distanceFromEpicenter,pWH,pAttacker,IgnoreDefenses,PreventPassengerEscape,pAttackingHouse) => { };


        static PartialHelper()
        {
            var type = typeof(TechnoGlobalExtension);
            var methods = type.GetMethods().Where(m => m.GetCustomAttributes()?.Count() > 0).ToList();

            foreach (var method in methods)
            {

                if (method.GetCustomAttribute(typeof(AwakeActionAttribute)) != null)
                {
                    List<ParameterExpression> parameterExpressions = new List<ParameterExpression>()
                    { };

                    ParameterExpression parameterExpression = Expression.Parameter(typeof(TechnoGlobalExtension), "owner");
                    MethodCallExpression methodCall = Expression.Call(parameterExpression, method, parameterExpressions);

                    var parameterExpressionAll = new List<ParameterExpression>()
                    { };
                    parameterExpressionAll.Add(parameterExpression);
                    parameterExpressionAll.AddRange(parameterExpressions);
                    Expression<Action<TechnoGlobalExtension>> expression = Expression.Lambda<Action<TechnoGlobalExtension>>
                       (methodCall, parameterExpressionAll);
                    var lambda = expression.Compile();
                    TechnoAwakeAction += lambda;
                }

                if (method.GetCustomAttribute(typeof(PutActionAttribute)) != null)
                {
                    List<ParameterExpression> parameterExpressions = new List<ParameterExpression>()
                    { Expression.Parameter(typeof(CoordStruct), "coord"), Expression.Parameter(typeof(Direction), "faceDir") };

                    ParameterExpression parameterExpression = Expression.Parameter(typeof(TechnoGlobalExtension), "owner");
                    MethodCallExpression methodCall = Expression.Call(parameterExpression, method, parameterExpressions);

                    var parameterExpressionAll = new List<ParameterExpression>()
                    { };
                    parameterExpressionAll.Add(parameterExpression);
                    parameterExpressionAll.AddRange(parameterExpressions);
                    Expression<Action<TechnoGlobalExtension, CoordStruct, Direction>> expression = Expression.Lambda<Action<TechnoGlobalExtension, CoordStruct, Direction>>
                       (methodCall, parameterExpressionAll);
                    var lambda = expression.Compile();

                    TechnoPutAction += lambda;
                }

                if (method.GetCustomAttribute(typeof(UpdateActionAttribute)) != null)
                {
                    List<ParameterExpression> parameterExpressions = new List<ParameterExpression>()
                    { };

                    ParameterExpression parameterExpression = Expression.Parameter(typeof(TechnoGlobalExtension), "owner");
                    MethodCallExpression methodCall = Expression.Call(parameterExpression, method, parameterExpressions);

                    var parameterExpressionAll = new List<ParameterExpression>()
                    { };
                    parameterExpressionAll.Add(parameterExpression);
                    parameterExpressionAll.AddRange(parameterExpressions);
                    Expression<Action<TechnoGlobalExtension>> expression = Expression.Lambda<Action<TechnoGlobalExtension>>
                       (methodCall, parameterExpressionAll);
                    var lambda = expression.Compile();
                    TechnoUpdateAction += lambda;
                }


                if (method.GetCustomAttribute(typeof(RemoveActionAttribute)) != null)
                {
                    List<ParameterExpression> parameterExpressions = new List<ParameterExpression>()
                    { };

                    ParameterExpression parameterExpression = Expression.Parameter(typeof(TechnoGlobalExtension), "owner");
                    MethodCallExpression methodCall = Expression.Call(parameterExpression, method, parameterExpressions);

                    var parameterExpressionAll = new List<ParameterExpression>()
                    { };
                    parameterExpressionAll.Add(parameterExpression);
                    parameterExpressionAll.AddRange(parameterExpressions);
                    Expression<Action<TechnoGlobalExtension>> expression = Expression.Lambda<Action<TechnoGlobalExtension>>
                       (methodCall, parameterExpressionAll);
                    var lambda = expression.Compile();
                    TechnoRemoveAction += lambda;
                }

                if (method.GetCustomAttribute(typeof(FireActionAttribute)) != null)
                {
                    List<ParameterExpression> parameterExpressions = new List<ParameterExpression>()
                    { Expression.Parameter(typeof(Pointer<AbstractClass>), "pTarget"), Expression.Parameter(typeof(int), "weaponIndex") };

                    ParameterExpression parameterExpression = Expression.Parameter(typeof(TechnoGlobalExtension), "owner");
                    MethodCallExpression methodCall = Expression.Call(parameterExpression, method, parameterExpressions);

                    var parameterExpressionAll = new List<ParameterExpression>()
                    { };
                    parameterExpressionAll.Add(parameterExpression);
                    parameterExpressionAll.AddRange(parameterExpressions);
                    Expression<Action<TechnoGlobalExtension, Pointer<AbstractClass>, int>> expression = Expression.Lambda<Action<TechnoGlobalExtension, Pointer<AbstractClass>, int>>
                       (methodCall, parameterExpressionAll);
                    var lambda = expression.Compile();

                    TechnoFireAction += lambda;
                }

                if (method.GetCustomAttribute(typeof(ReceiveDamageActionAttribute)) != null)
                {
                    List<ParameterExpression> parameterExpressions = new List<ParameterExpression>()
                    {
                        Expression.Parameter(typeof(Pointer<int>), "pDamage"),
                        Expression.Parameter(typeof(int), "DistanceFromEpicenter"),
                        Expression.Parameter(typeof(Pointer<WarheadTypeClass>), "pWh"),
                        Expression.Parameter(typeof(Pointer<ObjectClass>), "pAttacker"),
                        Expression.Parameter(typeof(bool), "IgnoreDefenses"),
                        Expression.Parameter(typeof(bool), "PreventPassengerEscape"),
                        Expression.Parameter(typeof(Pointer<HouseClass>), "pAttackingHouse")
                    };

                    ParameterExpression parameterExpression = Expression.Parameter(typeof(TechnoGlobalExtension), "owner");
                    MethodCallExpression methodCall = Expression.Call(parameterExpression, method, parameterExpressions);

                    var parameterExpressionAll = new List<ParameterExpression>()
                    { };
                    parameterExpressionAll.Add(parameterExpression);
                    parameterExpressionAll.AddRange(parameterExpressions);
                    Expression<Action<TechnoGlobalExtension, Pointer<int>, int, Pointer<WarheadTypeClass>, Pointer<ObjectClass>, bool, bool, Pointer<HouseClass>>> expression = Expression.Lambda<Action<TechnoGlobalExtension, Pointer<int>, int, Pointer<WarheadTypeClass>, Pointer<ObjectClass>, bool, bool, Pointer<HouseClass>>>
                       (methodCall, parameterExpressionAll);
                    var lambda = expression.Compile();

                    TechnoReceiveDamageAction += lambda;
                }

            }
        }


    }
}
