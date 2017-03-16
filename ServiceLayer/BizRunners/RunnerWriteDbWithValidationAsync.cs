// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BizLogic.GenericInterfaces;
using DataLayer.EfCode;

namespace ServiceLayer.BizRunners
{
    public class RunnerWriteDbWithValidationAsync<TIn, TOut> //#A
    {
        private readonly IBizActionAsync<TIn, TOut> _actionClass; //#B
        private readonly EfCoreContext _context;

        public IImmutableList<ValidationResult>
            Errors { get; private set; }
        public bool HasErrors => Errors.Any();

        public RunnerWriteDbWithValidationAsync(
            IBizActionAsync<TIn, TOut> actionClass, //#C
            EfCoreContext context)
        {
            _context = context;
            _actionClass = actionClass;
        }

        public async Task<TOut> //#D
            RunActionAsync(TIn dataIn) //#E
        {
            var result = await //#F
                _actionClass.ActionAsync(dataIn) //#G
                    .ConfigureAwait(false); //#H
            Errors = _actionClass.Errors;
            if (!HasErrors)
            {
                Errors =  
                    (await _context //#I
                      .SaveChangesWithCheckingAsync() //#J
                      .ConfigureAwait(false)) //#K
                        .ToImmutableList();
            }
            return result;
        }
    }
    /*********************************************************
     * 
     * ******************************************************/
}