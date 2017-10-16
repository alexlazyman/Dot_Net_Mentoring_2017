using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace Task1.Tests
{
    [TestFixture]
    public class FileSystemVisitorTests
    {
        private const string StartPath = "top";
        private readonly Mock<IDirectoryReader> _directoryReaderMock = new Mock<IDirectoryReader>();

        private readonly Mock<Predicate<string>> _filterMock = new Mock<Predicate<string>>();
        private readonly Mock<EventHandler> _startEventHandlerMock = new Mock<EventHandler>();
        private readonly Mock<EventHandler> _finishEventHandlerMock = new Mock<EventHandler>();
        private readonly Mock<EventHandler<VisitArgs>> _fileFoundEventHandlerMock = new Mock<EventHandler<VisitArgs>>();
        private readonly Mock<EventHandler<VisitArgs>> _filteredFileFoundEventHandlerMock = new Mock<EventHandler<VisitArgs>>();
        private readonly Mock<EventHandler<VisitArgs>> _directoryFoundEventHandlerMock = new Mock<EventHandler<VisitArgs>>();
        private readonly Mock<EventHandler<VisitArgs>> _filteredDirectoryFoundEventHandlerMock = new Mock<EventHandler<VisitArgs>>();

        private FileSystemVisitor _fileSystemVisitor;


        [SetUp]
        public void SetUp()
        {
            _fileSystemVisitor = new FileSystemVisitor(StartPath, _directoryReaderMock.Object, _filterMock.Object);

            _fileSystemVisitor.Start += _startEventHandlerMock.Object;
            _fileSystemVisitor.Finish += _finishEventHandlerMock.Object;
            _fileSystemVisitor.FileFound += _fileFoundEventHandlerMock.Object;
            _fileSystemVisitor.FilteredFileFound += _filteredFileFoundEventHandlerMock.Object;
            _fileSystemVisitor.DirectoryFound += _directoryFoundEventHandlerMock.Object;
            _fileSystemVisitor.FilteredDirectoryFound += _filteredDirectoryFoundEventHandlerMock.Object;

            _filterMock.SetReturnsDefault(true);
        }

        [TearDown]
        public void TearDown()
        {
            _directoryReaderMock.Reset();

            _filterMock.Reset();
            _startEventHandlerMock.Reset();
            _finishEventHandlerMock.Reset();
            _fileFoundEventHandlerMock.Reset();
            _filteredFileFoundEventHandlerMock.Reset();
            _filteredFileFoundEventHandlerMock.ResetCalls();
            _filteredDirectoryFoundEventHandlerMock.Reset();
        }

        [Test]
        public void GetEnumerator_PathIsDirectory_ExpectedReturnsDirectory()
        {
            _directoryReaderMock.Setup(r => r.IsFile(StartPath)).Returns(false);
            _directoryReaderMock.Setup(r => r.IsDirectory(StartPath)).Returns(true);
            _directoryReaderMock.Setup(r => r.GetDirectories(StartPath)).Returns(NoDirs);
            _directoryReaderMock.Setup(r => r.GetFiles(StartPath)).Returns(NoFiles);

            var expectedItems = BaseItem;

            var actualItems = _fileSystemVisitor.ToArray();

            CollectionAssert.AreEqual(expectedItems, actualItems);
        }

        [Test]
        public void GetEnumerator_PathIsFile_ExpectedReturnsFile()
        {
            _directoryReaderMock.Setup(r => r.IsFile(StartPath)).Returns(true);
            _directoryReaderMock.Setup(r => r.IsDirectory(StartPath)).Returns(false);
            _directoryReaderMock.Setup(r => r.GetDirectories(StartPath)).Returns(NoDirs);
            _directoryReaderMock.Setup(r => r.GetFiles(StartPath)).Returns(NoFiles);

            var expectedItems = BaseItem;

            var actualItems = _fileSystemVisitor.ToArray();

            CollectionAssert.AreEqual(expectedItems, actualItems);
        }

        [Test]
        public void GetEnumerator_PathIsValid_ExpectedStartAndEndEvents()
        {
            _directoryReaderMock.Setup(r => r.IsFile(StartPath)).Returns(false);
            _directoryReaderMock.Setup(r => r.IsDirectory(StartPath)).Returns(true);
            _directoryReaderMock.Setup(r => r.GetDirectories(StartPath)).Returns(NoDirs);
            _directoryReaderMock.Setup(r => r.GetFiles(StartPath)).Returns(NoFiles);

            _startEventHandlerMock.Setup(h => h(It.IsAny<FileSystemVisitor>(), It.IsAny<EventArgs>())).Verifiable();
            _finishEventHandlerMock.Setup(h => h(It.IsAny<FileSystemVisitor>(), It.IsAny<EventArgs>())).Verifiable();

            var expectedItems = BaseItem;

            var actualItems = _fileSystemVisitor.ToArray();

            CollectionAssert.AreEqual(expectedItems, actualItems);
            _startEventHandlerMock.Verify();
            _finishEventHandlerMock.Verify();
        }

        [Test]
        public void GetEnumerator_PathHasFilteredFile_ExpectedFileFoundAndFilteredFileFoundEvents()
        {
            var files = Files(1);

            _directoryReaderMock.Setup(r => r.IsFile(StartPath)).Returns(false);
            _directoryReaderMock.Setup(r => r.IsDirectory(StartPath)).Returns(true);
            _directoryReaderMock.Setup(r => r.GetDirectories(StartPath)).Returns(NoDirs);
            _directoryReaderMock.Setup(r => r.GetFiles(StartPath)).Returns(files);

            _filterMock
                .Setup(h => h(It.Is<string>(path => path == files[0])))
                .Returns(true)
                .Verifiable();
            _fileFoundEventHandlerMock
                .Setup(h => h(It.IsAny<FileSystemVisitor>(), It.Is<VisitArgs>(args => args.Path == files[0])))
                .Verifiable();
            _filteredFileFoundEventHandlerMock
                .Setup(h => h(It.IsAny<FileSystemVisitor>(), It.Is<VisitArgs>(args => args.Path == files[0])))
                .Verifiable();

            var expectedItems = Enumerable.Concat(BaseItem, files);

            var actualItems = _fileSystemVisitor.ToArray();

            CollectionAssert.AreEqual(expectedItems, actualItems);
            _filterMock.Verify();
            _fileFoundEventHandlerMock.Verify();
            _filteredFileFoundEventHandlerMock.Verify();
        }

        [Test]
        public void GetEnumerator_PathHasNotFilteredFile_ExpectedFileFoundAndNoFilteredFileFoundEvents()
        {
            var files = Files(1);

            _directoryReaderMock.Setup(r => r.IsFile(StartPath)).Returns(false);
            _directoryReaderMock.Setup(r => r.IsDirectory(StartPath)).Returns(true);
            _directoryReaderMock.Setup(r => r.GetDirectories(StartPath)).Returns(NoDirs);
            _directoryReaderMock.Setup(r => r.GetFiles(StartPath)).Returns(files);

            _filterMock
                .Setup(h => h(It.Is<string>(path => path == files[0])))
                .Returns(false)
                .Verifiable();
            _fileFoundEventHandlerMock
                .Setup(h => h(It.IsAny<FileSystemVisitor>(), It.Is<VisitArgs>(args => args.Path == files[0])))
                .Verifiable();
            _filteredFileFoundEventHandlerMock
                .Setup(h => h(It.IsAny<FileSystemVisitor>(), It.Is<VisitArgs>(args => args.Path == files[0])))
                .Verifiable();

            var expectedItems = BaseItem;

            var actualItems = _fileSystemVisitor.ToArray();

            CollectionAssert.AreEqual(expectedItems, actualItems);
            _filterMock.Verify();
            _fileFoundEventHandlerMock.Verify();
            _filteredFileFoundEventHandlerMock.Verify(h => h(It.IsAny<FileSystemVisitor>(), It.IsAny<VisitArgs>()), Times.Never);
        }

        [Test]
        public void GetEnumerator_PathHasFilteredDirectory_ExpectedDirectoryFoundAndFilteredDirectoryFoundEvents()
        {
            var dirs = Dirs(1);

            _directoryReaderMock.Setup(r => r.IsFile(StartPath)).Returns(false);
            _directoryReaderMock.Setup(r => r.IsDirectory(StartPath)).Returns(true);
            _directoryReaderMock.Setup(r => r.GetDirectories(StartPath)).Returns(dirs);
            _directoryReaderMock.Setup(r => r.GetFiles(StartPath)).Returns(NoFiles);

            _filterMock
                .Setup(h => h(It.Is<string>(path => path == dirs[0])))
                .Returns(true)
                .Verifiable();
            _directoryFoundEventHandlerMock
                .Setup(h => h(It.IsAny<FileSystemVisitor>(), It.Is<VisitArgs>(args => args.Path == dirs[0])))
                .Verifiable();
            _filteredDirectoryFoundEventHandlerMock
                .Setup(h => h(It.IsAny<FileSystemVisitor>(), It.Is<VisitArgs>(args => args.Path == dirs[0])))
                .Verifiable();

            var expectedItems = Enumerable.Concat(BaseItem, dirs);

            var actualItems = _fileSystemVisitor.ToArray();

            CollectionAssert.AreEqual(expectedItems, actualItems);
            _filterMock.Verify();
            _directoryFoundEventHandlerMock.Verify();
            _filteredDirectoryFoundEventHandlerMock.Verify();
        }

        [Test]
        public void GetEnumerator_PathHasNotFilteredDirectory_ExpectedDirectoryFoundAndNoFilteredDirectoryFoundEvents()
        {
            var dirs = Dirs(1);

            _directoryReaderMock.Setup(r => r.IsFile(StartPath)).Returns(false);
            _directoryReaderMock.Setup(r => r.IsDirectory(StartPath)).Returns(true);
            _directoryReaderMock.Setup(r => r.GetDirectories(StartPath)).Returns(dirs);
            _directoryReaderMock.Setup(r => r.GetFiles(StartPath)).Returns(NoFiles);

            _filterMock
                .Setup(h => h(It.Is<string>(path => path == dirs[0])))
                .Returns(false)
                .Verifiable();
            _filteredDirectoryFoundEventHandlerMock
                .Setup(h => h(It.IsAny<FileSystemVisitor>(), It.Is<VisitArgs>(args => args.Path == dirs[0])))
                .Verifiable();
            _filteredFileFoundEventHandlerMock
                .Setup(h => h(It.IsAny<FileSystemVisitor>(), It.Is<VisitArgs>(args => args.Path == dirs[0])))
                .Verifiable();

            var expectedItems = BaseItem;

            var actualItems = _fileSystemVisitor.ToArray();

            CollectionAssert.AreEqual(expectedItems, actualItems);
            _filterMock.Verify();
            _directoryFoundEventHandlerMock.Verify();
            _filteredDirectoryFoundEventHandlerMock.Verify(h => h(It.IsAny<FileSystemVisitor>(), It.Is<VisitArgs>(args => args.Path == dirs[0])), Times.Never);
        }

        [Test]
        public void GetEnumerator_DirectoryHas3FilesAnd2ndFileStopsSearchOnFileFoundEvent_ExpectedReturns2Files()
        {
            var files = Files(3);

            _directoryReaderMock.Setup(r => r.IsFile(StartPath)).Returns(false);
            _directoryReaderMock.Setup(r => r.IsDirectory(StartPath)).Returns(true);
            _directoryReaderMock.Setup(r => r.GetDirectories(StartPath)).Returns(NoDirs);
            _directoryReaderMock.Setup(r => r.GetFiles(StartPath)).Returns(files);

            _fileFoundEventHandlerMock
                .Setup(h => h(It.IsAny<FileSystemVisitor>(), It.IsAny<VisitArgs>()))
                .Callback((object s, VisitArgs args) => args.StopSearch = args.Path == files[1]);

            var expectedItems = Enumerable.Concat(BaseItem, files.Take(2));

            var actualItems = _fileSystemVisitor.ToArray();

            CollectionAssert.AreEqual(expectedItems, actualItems);
        }

        [Test]
        public void GetEnumerator_DirectoryHas3FilesAnd2ndFileStopsSearchOnFilteredFileFoundEvent_ExpectedReturns2Files()
        {
            var files = Files(3);

            _directoryReaderMock.Setup(r => r.IsFile(StartPath)).Returns(false);
            _directoryReaderMock.Setup(r => r.IsDirectory(StartPath)).Returns(true);
            _directoryReaderMock.Setup(r => r.GetDirectories(StartPath)).Returns(NoDirs);
            _directoryReaderMock.Setup(r => r.GetFiles(StartPath)).Returns(files);

            _filteredFileFoundEventHandlerMock
                .Setup(h => h(It.IsAny<FileSystemVisitor>(), It.IsAny<VisitArgs>()))
                .Callback((object s, VisitArgs args) => args.StopSearch = args.Path == files[1]);

            var expectedItems = Enumerable.Concat(BaseItem, files.Take(2));

            var actualItems = _fileSystemVisitor.ToArray();

            CollectionAssert.AreEqual(expectedItems, actualItems);
        }

        [Test]
        public void GetEnumerator_DirectoryHas3DirectoryAnd2ndDirectoryStopsSearchOnDirectoryFoundEvent_ExpectedReturns1stDirectoryWithFilesAnd2ndDirectory()
        {
            var dirs = Dirs(3);
            var dir1Files = Files(3, 1);
            var dir2Files = Files(3, 2);
            var dir3Files = Files(3, 3);

            _directoryReaderMock.Setup(r => r.IsFile(StartPath)).Returns(false);
            _directoryReaderMock.Setup(r => r.IsDirectory(StartPath)).Returns(true);
            _directoryReaderMock.Setup(r => r.GetDirectories(StartPath)).Returns(dirs);
            _directoryReaderMock.Setup(r => r.GetFiles(dirs[0])).Returns(dir1Files);
            _directoryReaderMock.Setup(r => r.GetFiles(dirs[1])).Returns(dir2Files);
            _directoryReaderMock.Setup(r => r.GetFiles(dirs[2])).Returns(dir3Files);

            _directoryFoundEventHandlerMock
                .Setup(h => h(It.IsAny<FileSystemVisitor>(), It.IsAny<VisitArgs>()))
                .Callback((object s, VisitArgs args) => args.StopSearch = args.Path == dirs[1]);

            var expectedItems = new List<string>();
            expectedItems.AddRange(BaseItem);
            expectedItems.AddRange(dirs.Skip(0).Take(1));
            expectedItems.AddRange(dir1Files);
            expectedItems.AddRange(dirs.Skip(1).Take(1));

            var actualItems = _fileSystemVisitor.ToArray();

            CollectionAssert.AreEqual(expectedItems, actualItems);
        }

        [Test]
        public void GetEnumerator_DirectoryHas3DirectoryAnd2ndDirectoryStopsSearchOnFilteredDirectoryFoundEvent_ExpectedReturns1stDirectoryWithFilesAnd2ndDirectory()
        {
            var dirs = Dirs(3);
            var dir1Files = Files(3, 1);
            var dir2Files = Files(3, 2);
            var dir3Files = Files(3, 3);

            _directoryReaderMock.Setup(r => r.IsFile(StartPath)).Returns(false);
            _directoryReaderMock.Setup(r => r.IsDirectory(StartPath)).Returns(true);
            _directoryReaderMock.Setup(r => r.GetDirectories(StartPath)).Returns(dirs);
            _directoryReaderMock.Setup(r => r.GetFiles(dirs[0])).Returns(dir1Files);
            _directoryReaderMock.Setup(r => r.GetFiles(dirs[1])).Returns(dir2Files);
            _directoryReaderMock.Setup(r => r.GetFiles(dirs[2])).Returns(dir3Files);

            _filteredDirectoryFoundEventHandlerMock
                .Setup(h => h(It.IsAny<FileSystemVisitor>(), It.IsAny<VisitArgs>()))
                .Callback((object s, VisitArgs args) => args.StopSearch = args.Path == dirs[1]);

            var expectedItems = new List<string>();
            expectedItems.AddRange(BaseItem);
            expectedItems.AddRange(dirs.Skip(0).Take(1));
            expectedItems.AddRange(dir1Files);
            expectedItems.AddRange(dirs.Skip(1).Take(1));

            var actualItems = _fileSystemVisitor.ToArray();

            CollectionAssert.AreEqual(expectedItems, actualItems);
        }

        private string[] BaseItem => new [] { StartPath };

        private string[] NoDirs => new string[0];

        private string[] NoFiles => new string[0];

        private string[] Files(int count = 1, int level = 0) => Enumerable.Repeat("file", count).Select((t, i) => $"{t}_{level}_{i}").ToArray();

        private string[] Dirs(int count = 1, int level = 0) => Enumerable.Repeat("dir", count).Select((t, i) => $"{t}_{level}_{i}").ToArray();
    }
}
