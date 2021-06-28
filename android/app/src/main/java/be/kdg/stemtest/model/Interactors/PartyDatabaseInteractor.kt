package be.kdg.stemtest.model.Interactors

import be.kdg.stemtest.model.db.StemTestDb
import be.kdg.stemtest.model.entity.Party
import io.reactivex.Completable
import io.reactivex.Flowable
import io.reactivex.Maybe
import io.reactivex.Single
import io.reactivex.schedulers.Schedulers
import javax.inject.Inject

class PartyDatabaseInteractor @Inject constructor(private val stemTestDb: StemTestDb) {


    fun getChosenParty(): Single<Party> {
        return stemTestDb.partyDao().getChosenParty()
    }

    fun getAllParties(): Flowable<List<Party>> {
        var testList : List<Party> = listOf()

        return stemTestDb.partyDao().getAllParties()
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .doOnSuccess { i -> testList = i }
            .repeatUntil {
                testList.isNotEmpty()}
    }

    fun updateParty(chosenParty: Party): Completable {
       return stemTestDb.partyDao()
            .update(chosenParty)
    }

    fun getParty(partyName: String): Single<Party> {
       return stemTestDb.partyDao().getParty(partyName)
    }
}