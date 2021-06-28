package be.kdg.stemtest.model.db

import androidx.room.Database
import androidx.room.RoomDatabase
import androidx.room.TypeConverters
import be.kdg.stemtest.model.db.converters.Converters
import be.kdg.stemtest.model.db.dao.*
import be.kdg.stemtest.model.entity.*


@Database(entities = [StudentAnswer::class,
    AnswerOption::class,
    Statement::class,
    Identification::class,
    Party::class,
    PartyGameResult::class,
    PartyAnswer::class,
    GameSettings::class,
    DebateGameResult::class,
    Definition::class,
    CustomDebateGameResult::class], version = 44)

@TypeConverters(Converters::class)

abstract class StemTestDb : RoomDatabase() {
    abstract fun answerDao():AnswerDao
    abstract fun answerOptionDao():AnswerOptionDao
    abstract fun identificationDao():IdentificationDao
    abstract fun statementDao():StatementDao
    abstract fun partyDao():PartyDao
    abstract fun partyResultDao():PartyGameResultDao
    abstract fun settingsDao(): GameSettingsDao
    abstract fun partyAnswerDao(): PartyAnswerDao
    abstract fun debateGameDao(): DebateGameDao
    abstract fun definitionDao():DefinitionDao
    abstract fun customDebateGameDao() : CustomDebateGameDao
}