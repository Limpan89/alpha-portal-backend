﻿using Data.Contexts;
using Data.Entities;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Data.Repositories;

public interface IStatusRepository : IBaseRepository<StatusEntity, StatusModel> { }

public class StatusRepository(DataContext context) : BaseRepository<StatusEntity, StatusModel>(context), IStatusRepository { }
