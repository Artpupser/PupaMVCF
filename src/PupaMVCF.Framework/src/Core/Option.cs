using Microsoft.Extensions.Logging;

namespace PupaMVCF.Framework.Core;

public readonly record struct Option(bool Success) {
   private static readonly Option _fail = new(false);

   public static implicit operator bool(Option option) {
      return option.Success;
   }

   public static Option Fail() {
      return _fail;
   }

   public static Option FailLog(ILogger logger, string message) {
      logger.LogError(message);
      return _fail;
   }

   public static Option Ok() {
      return new Option(true);
   }

   public static Task<Option> OkTask() {
      return Task.FromResult(new Option(true));
   }
}

public readonly record struct Option<T>(bool Success, T Content) {
   private static readonly Option<T> _fail = new(false, default!);

   public static implicit operator bool(Option<T> option) {
      return option.Success;
   }

   public static Option<T> Fail() {
      return _fail;
   }

   public static Option<T> FailLog(ILogger logger, string message) {
      logger.LogError(message);
      return _fail;
   }

   public static Option<T> Ok(T content) {
      return new Option<T>(true, content);
   }

   public (bool success, T content) Tuple() {
      return (Success, Content);
   }

   public bool Out(out T content) {
      content = Content;
      return Success;
   }
}