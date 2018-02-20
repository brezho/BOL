using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace X.DataModel
{
    public interface IHaveXRules
    {
        IEnumerable<XRule> GetRules();
    }
    public static class IHaveValidationRulesExt
    {
        public static IEnumerable<XRule> GetBrokenRules(this IHaveXRules item)
        {
            return item.GetRules().Where(x => x.IsBroken());
        }
    }

    public class XRule
    {
        public string Description { get; protected set; }
        public string PropertyName { get; protected set; }
        protected virtual Func<bool> RuleDelegate { get; set; }

        private XRule(string propertyName, string brokenDescription, Func<bool> passIf)
        {
            Description = brokenDescription;
            PropertyName = propertyName ?? string.Empty;
            RuleDelegate = passIf;
        }

        public bool IsBroken()
        {
            return !RuleDelegate();
        }

        public override string ToString()
        {
            return Description ?? base.ToString();
        }

        public static XRule Create(string propertyName, string brokenDescription, Func<bool> passIf)
        {
            return new XRule(propertyName, brokenDescription, passIf);
        }

        public static XRule Create<U>(Expression<Func<U>> getter, string brokenDescription, Func<bool> passIf)
        {
            return new XRule(getter.ToPropertyName(), brokenDescription, passIf);
        }

        public static XRule ValidIdentifier(Expression<Func<string>> getter)
        {
            return Create<string>(getter, "{0} is not a valid identifier", () => Compiler.IsValidReference(getter.Compile()()));
        }

        public static XRule MustSet(Expression<Func<string>> getter)
        {
            return Create<string>(getter, "{0} must be defined", () => !string.IsNullOrWhiteSpace(getter.Compile()()));
        }

        public static XRule Uniqueness<U>(Expression<Func<IEnumerable<U>>> listGetter, Func<U, string> itemGetter)
        {
            return XRule.Create(listGetter, "References are not unique in list {0}", () =>
            {
                return listGetter.Compile()().Aggregate(new Dictionary<string, int>(), (x, y) =>
                {
                    if (x.ContainsKey(itemGetter(y).Safe())) x[itemGetter(y).Safe()]++;
                    else x[itemGetter(y).Safe()] = 0;
                    return x;
                },
                (x) => x).All(x => x.Value == 0);
            });
        }
    }
}
