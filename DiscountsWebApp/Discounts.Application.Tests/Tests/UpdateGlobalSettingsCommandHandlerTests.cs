// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Admin.Command.UpdateGlobalSettings;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using MediatR;
using Moq;

namespace Discounts.Application.Tests.Admin;

public class UpdateGlobalSettingsCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IGlobalSettingRepository> _globalSettingsMock = new();
    private readonly UpdateGlobalSettingsCommandHandler _handler;

    public UpdateGlobalSettingsCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.GlobalSettings).Returns(_globalSettingsMock.Object);
        _handler = new UpdateGlobalSettingsCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenSettingDoesNotExist_ShouldCreateNewSetting()
    {
        var command = new UpdateGlobalSettingsCommand("SomeKey", "SomeValue");
        _globalSettingsMock.Setup(x => x.GetByKeyAsync(command.Key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GlobalSetting?)null);
        _globalSettingsMock.Setup(x => x.AddAsync(It.IsAny<GlobalSetting>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GlobalSetting());
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        _globalSettingsMock.Verify(x => x.AddAsync(
            It.Is<GlobalSetting>(s => s.Key == command.Key && s.Value == command.Value),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handle_WhenSettingExists_ShouldUpdateExistingSetting()
    {
        var existingSetting = new GlobalSetting { Id = Guid.NewGuid(), Key = "SomeKey", Value = "OldValue" };
        var command = new UpdateGlobalSettingsCommand("SomeKey", "NewValue");
        _globalSettingsMock.Setup(x => x.GetByKeyAsync(command.Key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSetting);
        _globalSettingsMock.Setup(x => x.Update(It.IsAny<GlobalSetting>()));
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal("NewValue", existingSetting.Value);
        _globalSettingsMock.Verify(x => x.Update(existingSetting), Times.Once);
        _globalSettingsMock.Verify(x => x.AddAsync(It.IsAny<GlobalSetting>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal(Unit.Value, result);
    }
}
