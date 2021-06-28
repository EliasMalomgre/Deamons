package be.kdg.stemtest.model.Interactors

import be.kdg.stemtest.model.db.StemTestDb
import be.kdg.stemtest.model.entity.Definition
import be.kdg.stemtest.model.entity.GameSettings
import be.kdg.stemtest.model.entity.Identification
import be.kdg.stemtest.model.entity.Party
import io.reactivex.Completable
import io.reactivex.Flowable
import io.reactivex.Maybe
import io.reactivex.android.schedulers.AndroidSchedulers
import io.reactivex.schedulers.Schedulers
import javax.inject.Inject

class ConnectionDatabaseInteractor @Inject constructor(private val stemTestDb: StemTestDb) {

    fun getData(): Maybe<Identification> {
       return stemTestDb.identificationDao().getIdentification()
    }

    fun saveData(identification: Identification): Completable {
        return stemTestDb.identificationDao().insert(identification)
    }

    fun insertAllDefinitions(definitions: List<Definition>) {
        stemTestDb.definitionDao().insertAllDefinitions(definitions)
    }

    fun insertSettings(i: GameSettings): Completable {
       return stemTestDb.settingsDao().insert(i)
    }

    fun saveParties(parties: List<Party>) {
        stemTestDb.partyDao().insertAllParties(parties)
    }

    fun getGameSettings(): Flowable<GameSettings> {
        var gameSettings : GameSettings?=null
        return stemTestDb.settingsDao()
            .getSettings()
            .doOnSuccess { i -> gameSettings=i }
            .repeatUntil { gameSettings!=null }
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
    }

    fun getIdentification(): Flowable<Identification> {
        var identification : Identification?=null

         return stemTestDb.identificationDao()
            .getIdentification()
            .doOnSuccess { i -> identification=i }
            .repeatUntil { identification!=null }
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
    }
}