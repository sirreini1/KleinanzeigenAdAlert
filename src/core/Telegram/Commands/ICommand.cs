using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KleinanzeigenAdAlert.core.Telegram.Commands;

public interface ICommand
{
    public Task HandleCommand(Message msg, UpdateType updateType);
}