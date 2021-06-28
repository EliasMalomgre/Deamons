package be.kdg.stemtest.DI.modules

import android.content.Context
import androidx.room.Room
import be.kdg.stemtest.model.db.StemTestDb
import dagger.Module
import dagger.Provides
import javax.inject.Singleton

@Module
class DatabaseModule {

    @Singleton
    @Provides
    fun provideDatabase(context: Context): StemTestDb{
        return Room.databaseBuilder(context,
            StemTestDb::class.java,"StemtestDatabase")
            .fallbackToDestructiveMigration()
            .build()
    }


    }