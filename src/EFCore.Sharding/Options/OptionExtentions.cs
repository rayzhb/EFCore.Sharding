﻿using Microsoft.Extensions.Options;
using System;

namespace EFCore.Sharding
{
    internal static class OptionExtentions
    {
        public static EFCoreShardingOptions BuildOption(this IOptionsMonitor<EFCoreShardingOptions> optionsSnapshot, string optionName)
        {
            if (optionName.IsNullOrEmpty())
            {
                return optionsSnapshot.CurrentValue;
            }
            else
            {
                var selfOption = optionsSnapshot.Get(optionName);
                var defaultOption = new EFCoreShardingOptions();
                var globalOption = optionsSnapshot.CurrentValue;
                foreach (var aProperty in typeof(EFCoreShardingOptions).GetProperties())
                {
                    var selfValue = aProperty.GetValue(selfOption);
                    var defaultValue = aProperty.GetValue(defaultOption);
                    var globalValue = aProperty.GetValue(globalOption);

                    var value = Equals(selfValue, defaultValue) ? globalValue : selfValue;
                    aProperty.SetValue(selfOption, value);
                }

                if (selfOption.Types.Length == 0)
                {
                    selfOption.Types = globalOption.Types;
                }

                if (selfOption.Types.Length == 0)
                {
                    throw new Exception("EFCore.Sharding:请配置EFCoreShardingOptions.EntityAssemblies指定实体程序集");
                }

                return selfOption;
            }
        }

    }
}
