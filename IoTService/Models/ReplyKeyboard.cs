using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoTService.Models
{
    public interface IKeyboard
    {
        
        
        public static IKeyboard Remove => new RemoveKeyboard();

       


    }

    public class ReplyKeyboard : IKeyboard
    {
        public IEnumerable<IEnumerable<ReplyKeyboardKey>> keyboard { get; set; }

        public static ReplyKeyboard FromStringKeysVertical(IEnumerable<string> keys)
        {
            return FromStringKeys(keys.Select(s => new[] { s }));
        }
        public static ReplyKeyboard FromStringKeys(IEnumerable<IEnumerable<string>> keys)
        {
            return new ReplyKeyboard()
            {
                keyboard = keys.Select(s => s.Select(ins => new ReplyKeyboardKey() { text = ins }))
            };
        }
    }

    public class InlineReplyKeyboard: IKeyboard
    {
        public IEnumerable<IEnumerable<InlineReplyKeyboardKey>> inline_keyboard { get; set; }


        public static InlineReplyKeyboard FromStringKeys(IEnumerable<IEnumerable<string>> keys)
        {
            return new InlineReplyKeyboard()
            {
                inline_keyboard = keys.Select(s => s.Select(ins => new InlineReplyKeyboardKey() { text = ins }))
            };
        }
    }

    public class RemoveKeyboard : IKeyboard
    {
        public bool remove_keyboard { get; set; } = true;
    }



    public class ReplyKeyboardKey
    {
        public string text { get; set; }
    }

    public class InlineReplyKeyboardKey
    {
        public string text { get; set; }
    }
}
