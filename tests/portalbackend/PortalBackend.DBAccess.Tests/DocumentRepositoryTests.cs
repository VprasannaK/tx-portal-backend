﻿/********************************************************************************
 * Copyright (c) 2021, 2023 BMW Group AG
 * Copyright (c) 2021, 2023 Contributors to the Eclipse Foundation
 *
 * See the NOTICE file(s) distributed with this work for additional
 * information regarding copyright ownership.
 *
 * This program and the accompanying materials are made available under the
 * terms of the Apache License, Version 2.0 which is available at
 * https://www.apache.org/licenses/LICENSE-2.0.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 * SPDX-License-Identifier: Apache-2.0
 ********************************************************************************/

using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Repositories;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Tests.Setup;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities;
using Org.Eclipse.TractusX.Portal.Backend.PortalBackend.PortalEntities.Enums;
using System.Text;
using Xunit;
using Xunit.Extensions.AssemblyFixture;

namespace Org.Eclipse.TractusX.Portal.Backend.PortalBackend.DBAccess.Tests;

/// <summary>
/// Tests the functionality of the <see cref="DocumentRepositoryTests"/>
/// </summary>
public class DocumentRepositoryTests : IAssemblyFixture<TestDbFixture>
{
    private readonly TestDbFixture _dbTestDbFixture;

    public DocumentRepositoryTests(TestDbFixture testDbFixture)
    {
        var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));

        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _dbTestDbFixture = testDbFixture;
    }

    #region Create Document

    [Fact]
    public async Task CreateDocument_ReturnsExpectedDocument()
    {
        // Arrange
        var (sut, context) = await CreateSut().ConfigureAwait(false);
        var test = "This is just test content";
        var content = Encoding.UTF8.GetBytes(test);

        // Act
        var result = sut.CreateDocument("New Document", content, content, DocumentTypeId.APP_CONTRACT, doc =>
        {
            doc.DocumentStatusId = DocumentStatusId.INACTIVE;
        });

        // Assert
        var changeTracker = context.ChangeTracker;
        var changedEntries = changeTracker.Entries().ToList();
        result.DocumentTypeId.Should().Be(DocumentTypeId.APP_CONTRACT);
        result.DocumentStatusId.Should().Be(DocumentStatusId.INACTIVE);
        changeTracker.HasChanges().Should().BeTrue();
        changedEntries.Should().NotBeEmpty();
        changedEntries.Should().HaveCount(1);
        var changedEntity = changedEntries.Single();
        changedEntity.State.Should().Be(EntityState.Added);
    }
    
    #endregion

    #region GetUploadedDocuments

    [Theory]
    [InlineData("6b2d1263-c073-4a48-bfaf-704dc154ca9c", DocumentTypeId.CX_FRAME_CONTRACT, "555b0b81-6ead-4d3d-8d5d-41c07bb8cfbb", 2)]
    [InlineData("6b2d1263-c073-4a48-bfaf-704dc154ca9f", DocumentTypeId.CX_FRAME_CONTRACT, "623770c5-cf38-4b9f-9a35-f8b9ae972e2d", 1)]
    [InlineData("6b2d1263-c073-4a48-bfaf-704dc154ca9c", DocumentTypeId.APP_CONTRACT, "4a23930a-30b6-461c-9ad4-58d3e761a0b5", 0)]
    public async Task GetUploadedDocumentsAsync_ReturnsExpectedDocuments(Guid applicationId, DocumentTypeId documentTypeId, string iamUserId, int count)
    {
        // Arrange
        var (sut, _) = await CreateSut().ConfigureAwait(false);
    
        // Act
        var results = await sut.GetUploadedDocumentsAsync(applicationId, documentTypeId, iamUserId).ConfigureAwait(false);
    
        // Assert
        results.Should().NotBe(default);
        results.IsApplicationAssignedUser.Should().BeTrue();
        results.Documents.Should().HaveCount(count);
    }

    [Fact]
    public async Task GetUploadedDocumentsAsync_InvalidApplication_ReturnsDefault()
    {
        // Arrange
        var (sut, _) = await CreateSut().ConfigureAwait(false);
    
        // Act
        var result = await sut.GetUploadedDocumentsAsync(Guid.NewGuid(), DocumentTypeId.CX_FRAME_CONTRACT, "623770c5-cf38-4b9f-9a35-f8b9ae972e2e").ConfigureAwait(false);

        // Assert
        result.Should().Be(default);
    }

    [Fact]
    public async Task GetUploadedDocumentsAsync_InvalidUser_ReturnsExpected()
    {
        // Arrange
        var (sut, _) = await CreateSut().ConfigureAwait(false);
    
        // Act
        var result = await sut.GetUploadedDocumentsAsync(new Guid("4f0146c6-32aa-4bb1-b844-df7e8babdcb3"), DocumentTypeId.CX_FRAME_CONTRACT, Guid.NewGuid().ToString()).ConfigureAwait(false);

        // Assert
        result.Should().NotBe(default);
        result.IsApplicationAssignedUser.Should().BeFalse();
        result.Documents.Should().BeEmpty();
    }

    #endregion

    #region GetDocumentDataAndIsCompanyUserAsync_ReturnsExpectedDocuments

    [Fact]
    public async Task GetDocumentDataAndIsCompanyUserAsync_WithValidData_ReturnsExpectedDocument()
    {
        // Arrange
        var (sut, _) = await CreateSut().ConfigureAwait(false);
    
        // Act
        var result = await sut.GetDocumentDataAndIsCompanyUserAsync(new Guid("00000000-0000-0000-0000-000000000001"), "502dabcf-01c7-47d9-a88e-0be4279097b5").ConfigureAwait(false);
    
        // Assert
        result.Should().NotBe(default);
        result.FileName.Should().Be("Default_App_Image");
        result.IsUserInCompany.Should().BeTrue();
    }

    [Fact]
    public async Task GetDocumentDataAndIsCompanyUserAsync_WithWrongUserData_ReturnsIsUserInCompanyFalse()
    {
        // Arrange
        var (sut, _) = await CreateSut().ConfigureAwait(false);
    
        // Act
        var result = await sut.GetDocumentDataAndIsCompanyUserAsync(new Guid("00000000-0000-0000-0000-000000000001"), "4a23930a-30b6-461c-9ad4-58d3e761a0b5").ConfigureAwait(false);

        // Assert
        result.Should().NotBe(default);
        result.FileName.Should().Be("Default_App_Image");
        result.IsUserInCompany.Should().BeFalse();
    }

    [Fact]
    public async Task GetDocumentDataAndIsCompanyUserAsync_WithNotExistingDocument_ReturnsDefault()
    {
        // Arrange
        var (sut, _) = await CreateSut().ConfigureAwait(false);
    
        // Act
        var result = await sut.GetDocumentDataAndIsCompanyUserAsync(Guid.NewGuid(), "502dabcf-01c7-47d9-a88e-0be4279097b5").ConfigureAwait(false);

        // Assert
        result.Should().Be(default);
    }

    #endregion

    #region GetDocumentDataAndIsCompanyUserAsync_ReturnsExpectedDocuments

    [Fact]
    public async Task GetDocumentDataByIdAndTypeAsync_WithValidData_ReturnsExpectedDocument()
    {
        // Arrange
        var (sut, _) = await CreateSut().ConfigureAwait(false);
    
        // Act
        var result = await sut.GetDocumentDataByIdAndTypeAsync(new Guid("00000000-0000-0000-0000-000000000001"), DocumentTypeId.APP_LEADIMAGE).ConfigureAwait(false);
    
        // Assert
        result.Should().NotBe(default);
        result.FileName.Should().Be("Default_App_Image");
    }

    [Fact]
    public async Task GetDocumentDataByIdAndTypeAsync_WithWrongType_ReturnsDefault()
    {
        // Arrange
        var (sut, _) = await CreateSut().ConfigureAwait(false);
    
        // Act
        var result = await sut.GetDocumentDataByIdAndTypeAsync(new Guid("00000000-0000-0000-0000-000000000001"), DocumentTypeId.APP_IMAGE).ConfigureAwait(false);

        // Assert
        result.Should().Be(default);
    }

    [Fact]
    public async Task GetDocumentDataByIdAndTypeAsync_WithNotExistingDocument_ReturnsDefault()
    {
        // Arrange
        var (sut, _) = await CreateSut().ConfigureAwait(false);
    
        // Act
        var result = await sut.GetDocumentDataByIdAndTypeAsync(Guid.NewGuid(), DocumentTypeId.APP_LEADIMAGE).ConfigureAwait(false);

        // Assert
        result.Should().Be(default);
    }

    #endregion

    #region AttachAndModifyDocument

    [Fact]
    public async Task AttachAndModifyDocument_ReturnsExpectedResult()
    {
        // Arrange
        var (sut, context) = await CreateSut().ConfigureAwait(false);

        // Act
        sut.AttachAndModifyDocument(Guid.NewGuid(),
            null,
            docstatusId =>{ docstatusId.DocumentStatusId = DocumentStatusId.LOCKED; });

        // Assert
        var changeTracker = context.ChangeTracker;
        var changedEntries = changeTracker.Entries().ToList();
        changeTracker.HasChanges().Should().BeTrue();
        changedEntries.Should().NotBeEmpty();
        changedEntries.Should().HaveCount(1);
        changedEntries.Single().Entity.Should()
            .BeOfType<PortalEntities.Entities.Document>()
                .Which.DocumentStatusId.Should().Be(DocumentStatusId.LOCKED);
    }

    [Fact]
    public async Task AttachAndModifyDocument_NoUpdate()
    {
        // Arrange
        var (sut, context) = await CreateSut().ConfigureAwait(false);

        // Act
        sut.AttachAndModifyDocument(Guid.NewGuid(),
            docstatusId =>{ docstatusId.DocumentStatusId = DocumentStatusId.LOCKED; },
            docstatusId =>{ docstatusId.DocumentStatusId = DocumentStatusId.LOCKED; });

        // Assert
        var changeTracker = context.ChangeTracker;
        var changedEntries = changeTracker.Entries().ToList();
        changeTracker.HasChanges().Should().BeFalse();
    }

    #endregion

    #region GetDocumentSeedDataByIdAsync

    [Fact]
    public async Task GetDocumentSeedDataByIdAsync_ReturnsExpectedDocuments()
    {
        // Arrange
        var (sut, _) = await CreateSut().ConfigureAwait(false);
    
        // Act
        var results = await sut.GetDocumentSeedDataByIdAsync(new Guid("00000000-0000-0000-0000-000000000001")).ConfigureAwait(false);
    
        // Assert
        results.Should().NotBeNull();
        results!.DocumentStatusId.Should().Be(2);
        results.DocumentTypeId.Should().Be(6);
        results.DocumentName.Should().Be("Default_App_Image");
    }

    [Fact]
    public async Task GetDocumentSeedDataByIdAsync_NotExistingId_ReturnsNull()
    {
        // Arrange
        var (sut, _) = await CreateSut().ConfigureAwait(false);
    
        // Act
        var result = await sut.GetDocumentSeedDataByIdAsync(Guid.NewGuid()).ConfigureAwait(false);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    
    #region GetOfferImageDocumentContentAsync

    [Theory]
    [InlineData("ac1cf001-7fbc-1f2f-817f-bce0572c0007", "fda6c9cb-62be-4a98-99c1-d9c5a2df4aaa", new [] { DocumentTypeId.APP_IMAGE,DocumentTypeId.APP_CONTRACT }, OfferTypeId.APP, true, true, true, true, false)]
    [InlineData("ac1cf001-7fbc-1f2f-817f-bce0572c0007", "246e3b20-899e-40d2-8d0d-cd9b4ced332c", new [] { DocumentTypeId.APP_IMAGE,DocumentTypeId.CX_FRAME_CONTRACT }, OfferTypeId.SERVICE, true, true, true, false, false)]
    [InlineData("ac1cf001-7fbc-1f2f-817f-bce0572c0007", "fda6c9cb-62be-4a98-99c1-d9c5a2df4aac", new [] { DocumentTypeId.APP_CONTRACT }, OfferTypeId.APP, true, true, false, true, false)]
    [InlineData("ac1cf001-7fbc-1f2f-817f-bce0572c0007", "d6eb6ec2-24a6-40c5-becb-2142c62fb117", new [] { DocumentTypeId.APP_IMAGE,DocumentTypeId.APP_LEADIMAGE }, OfferTypeId.APP, true, false, true, false, false)]
    [InlineData("99C5FD12-8085-4DE2-ABFD-215E1EE4BAA4", "deadbeef-0000-0000-0000-000000000000", new [] { DocumentTypeId.APP_IMAGE,DocumentTypeId.APP_LEADIMAGE }, OfferTypeId.APP, false, false, false, false, false)]
    public async Task GetOfferImageDocumentContentAsync_ReturnsExpectedResult(Guid offerId, Guid documentId, IEnumerable<DocumentTypeId> documentTypeIds, OfferTypeId offerTypeId, bool isDocumentExisting, bool isLinkedToOffer, bool isValidDocumentType, bool isValidOfferType, bool isInactive)
    {
        // Arrange
        var (sut, _) = await CreateSut().ConfigureAwait(false);
    
        // Act
        var result = await sut.GetOfferDocumentContentAsync(offerId, documentId, documentTypeIds, offerTypeId, CancellationToken.None).ConfigureAwait(false);

        if (isDocumentExisting && isLinkedToOffer && isValidDocumentType && isValidOfferType && !isInactive)
        {
            result.Content.Should().NotBeNull();
        }
        else
        {
            result.Content.Should().BeNull();
        }

        // Assert
        result.IsDocumentExisting.Should().Be(isDocumentExisting);
        result.IsDocumentLinkedToOffer.Should().Be(isLinkedToOffer);
        result.IsValidDocumentType.Should().Be(isValidDocumentType);
        result.IsValidOfferType.Should().Be(isValidOfferType);
        result.IsInactive.Should().Be(isInactive);
    }

    #endregion

    #region GetAppDocumentsAsync

    [Theory]
    [InlineData("31d4f694-a97d-46a7-aec0-e70141bf5402","2f1efa5c-60bd-405b-95ac-365009d7f3fd", new [] { DocumentTypeId.APP_IMAGE,DocumentTypeId.APP_CONTRACT }, OfferTypeId.APP)]
    public async Task GetAppDocumentsAsync_ReturnsExpectedResult(Guid documentId, string iamUserId, IEnumerable<DocumentTypeId> documentTypeIds, OfferTypeId offerTypeId)
    {
        // Arrange
        var (sut, _) = await CreateSut().ConfigureAwait(false);
    
        // Act
        var result = await sut.GetAppDocumentsAsync(documentId,iamUserId, documentTypeIds, offerTypeId).ConfigureAwait(false);

        // Assert
        result.Should().NotBeNull();
        result.OfferData.Should().ContainSingle();
        result.IsDocumentTypeMatch.Should().Be(true);
        var offer = result.OfferData.Single();
        offer.OfferId.Should().Be(new Guid("5cf74ef8-e0b7-4984-a872-474828beb510"));
    }
    
    #endregion
    
    [Fact]
    public async Task RemovedDocuments_WithExisting_Document()
    {
        // Arrange
        var (sut, context) = await CreateSut().ConfigureAwait(false);
        
        IEnumerable<Guid> documentIds = new [] { (new Guid("184cde16-52d4-4865-81f6-b5b45e3c9051")) };
        // Act
        sut.RemoveDocuments(documentIds);

        // Assert
        var changeTracker = context.ChangeTracker;
        var changedEntries = changeTracker.Entries().ToList();
        changeTracker.HasChanges().Should().BeTrue();
        changedEntries.Should().NotBeEmpty();
        changedEntries.Should().HaveCount(1);
        var changedEntity = changedEntries.Single();
        changedEntity.State.Should().Be(EntityState.Deleted);
    }

    #region Setup    

    private async Task<(DocumentRepository, PortalDbContext)> CreateSut()
    {
        var context = await _dbTestDbFixture.GetPortalDbContext().ConfigureAwait(false);
        var sut = new DocumentRepository(context);
        return (sut, context);
    }

    #endregion
}